using System;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using String = ProjectCore.Variables.String;

namespace ProjectCore.SocialFeature.Internal
{
    [CreateAssetMenu(fileName = "NakamaServer", menuName = "ProjectCore/SocialFeature/Cloud/NakamaServer")]
    [InlineEditor]
    public class NakamaServer : ScriptableObject
    {
        [SerializeField] private String AuthToken;
        [SerializeField] private String RefreshToken;

        [SerializeField] private GameEvent NakamaServerConnected;

        [SerializeField] private SocialFeatureConfig SocialFeatureConfig;

        [SerializeField] private bool DebugMode;

        public IClient Client { get; private set; }
        public ISocket Socket { get; private set; }
        public ISession Session { get; private set; }
        public IApiAccount Account { get; private set; }
        public IApiUser User => Account?.User;

        private IUserProgressRepository _progressRepository;

        public void Initialize()
        {
            InitializeNakamaServer();
        }

        private void InitializeNakamaServer()
        {
            var environment = SocialFeatureConfig.GeneralSettings.Environment;
            var data = SocialFeatureConfig.GetLocalServerData();

            switch (environment)
            {
                case Environment.Live:
                    data = SocialFeatureConfig.GetLiveServerData();
                    break;
                case Environment.Dev:
                    data = SocialFeatureConfig.GetDevServerData();
                    break;
                case Environment.Local:
                    data = SocialFeatureConfig.GetLocalServerData();
                    break;
            }

            Client = new Client(data.Scheme, data.Host, data.Port, data.Key)
            {
                GlobalRetryConfiguration = SocialFeatureConfig.GetRetryConfiguration(),
                Timeout = SocialFeatureConfig.GeneralSettings.RetryTimeOut
            };

            Socket = Client.NewSocket();

            Socket.Connected += () => { Print("[Nakama] Socket Connected"); };

            Socket.Closed += () => { Print("[Nakama] Socket Disconnected"); };
        }

        public async Task<ISession> Authenticate(
            IAuthStrategy strategy,
            Func<bool, ApiResponseException, ISession, Task> callback = null
        )
        {
            Print($"[Nakama] Authenticating with {strategy.GetType().Name}");

            if (Session != null)
                await KillSession(Session);

            try
            {
                ISession session = await strategy.Authenticate(Client, SocialFeatureConfig);
                await UpdateSession(session);
                NakamaServerConnected.Invoke();
                Print($"[Nakama] Authenticated with {strategy.GetType().Name}");
                callback?.Invoke(true, null, session);
                return session;
            }
            catch (ApiResponseException ex)
            {
                Print($"[Nakama] Failed to authenticate: {ex.Message}", logType: LogType.Error);
                callback?.Invoke(false, ex, null);
                return null;
            }
            catch (Exception ex)
            {
                Print($"[Nakama] Unexpected error: {ex.Message}", logType: LogType.Error);
                callback?.Invoke(false, null, null);
                return null;
            }
        }

        public async Task Link(ILinkStrategy strategy, Func<bool, ApiResponseException, Task> callback = null)
        {
            Print($"[Nakama] Linking device with {strategy.GetType().Name}");
            try
            {
                await strategy.Link(Session, Client, SocialFeatureConfig);
                callback?.Invoke(true, null);
            }
            catch (ApiResponseException ex)
            {
                Print($"[Nakama] Failed to link device: {ex.Message}", logType: LogType.Error);
                callback?.Invoke(false, ex);
            }
        }
        
        public async Task Unlink(IUnlinkStrategy strategy, Func<bool, ApiResponseException, Task> callback = null)
        {
            Print($"[Nakama] Unlinking device with {strategy.GetType().Name}");
            try
            {
                await strategy.Unlink(Session, Client, SocialFeatureConfig);
                callback?.Invoke(true, null);
            }
            catch (ApiResponseException ex)
            {
                Print($"[Nakama] Failed to unlink device: {ex.Message}", logType: LogType.Error);
                callback?.Invoke(false, ex);
            }
        }

        #region Helper Functions

        public async Task UpdateSession(ISession session)
        {
            Session = session;
            await Socket.ConnectAsync(Session, true, 10);
            await UpdateAccount();
        }

        public async Task KillSession(ISession session)
        {
            try
            {
                await Client.SessionLogoutAsync(session);
            }
            catch (ApiResponseException e)
            {
                Print("[Nakama] Failed to kill session: ", e.Message);
            }
        }
        
        public async Task DeleteAccount(ISession session = null)
        {
            session ??= Session;
            try
            {
                await Client.DeleteAccountAsync(session, SocialFeatureConfig.GetRetryConfiguration());
                Print("[Nakama] Deleted account]");
            }
            catch (ApiResponseException e)
            {
                Print("[Nakama] Failed to delete account: ", e.Message, LogType.Error);
            }
            await KillSession(session);
        }

        private void Print(string message, string extraInfo = "", LogType logType = LogType.Log)
        {
            if (!DebugMode) return;
            switch (logType)
            {
                case LogType.Log:
                    Debug.Log(string.Concat(message, extraInfo));
                    break;
                case LogType.Error:
                    Debug.LogError(string.Concat(message, extraInfo));
                    break;
                case LogType.Warning:
                    Debug.LogWarning(string.Concat(message, extraInfo));
                    break;
            }
        }

        private async Task UpdateAccount()
        {
            try
            {
                Account = await Client.GetAccountAsync(Session);
                Print("[Nakama Account Fetched: ", User.Id);
            }
            catch (ApiResponseException e)
            {
                Print("[Nakama] Failed to get account ID: ", e.Message);
            }
        }

        public async Task<bool> ValidateSession(bool forceSync = false)
        {
            if (Session == null)
            {
                Print("[Nakama] Session is Invalid!!", logType: LogType.Error);

                Session = global::Nakama.Session.Restore(AuthToken, RefreshToken);
                if (Session == null)
                {
                    Print("[Nakama] Failed to restore session", logType: LogType.Error);
                    return false;
                }
                var isValidSocket = await ValidateSocket();
                return isValidSocket;
            }

            if ((Session.IsExpired && Session.IsRefreshExpired) || forceSync)
            {
                Print("[Nakama] Session is Invalid!!", logType: LogType.Error);
                try
                {
                    Session = await Client.SessionRefreshAsync(Session);

                    if (Session != null) return await ValidateSocket();
                    
                    Print("[Nakama] Failed to refresh session", logType: LogType.Error);
                    return false;
                }
                catch (ApiResponseException ex)
                {
                    Print("[Nakama] Failed to validate session: ", ex.Message, LogType.Error);
                    return false;
                }
            }
            
            Print("[Nakama] Session is Valid!!");
            SaveSession();
            return true;
        }

        private async Task<bool> ValidateSocket()
        {
            if (!Socket.IsConnected)
            {
                Print("[Nakama] Socket is Invalid!!", logType: LogType.Error);
                try
                {
                    await Socket.ConnectAsync(Session, SocialFeatureConfig.CanAppearOnline());
                    return true;
                }
                catch (ApiResponseException exception)
                {
                    Print("[Nakama] Socket Connection Failed: " ,exception.Message, LogType.Error);
                    return false;
                }
            }
            Print("[Nakama] Socket is Valid!!");
            return true;
        }

        private void SaveSession()
        {
            AuthToken.SetValue(Session.AuthToken);
            RefreshToken.SetValue(Session.RefreshToken);
        }

        #endregion

        #region UserProgressStorage

        // https://heroiclabs.com/docs/nakama/concepts/storage/collections
        
        public async Task SaveUserDataAsync(IApiWriteStorageObject[] objects)
        {
            try
            {
                await _progressRepository.SaveAsync(objects);
                Print("[Nakama] Saved user data");
            }
            catch (ApiResponseException ex)
            {
                Print($"[Nakama] Save failed: {ex.Message}", logType: LogType.Error);
            }
        }

        public async Task<IApiStorageObjects> GetUserDataAsync(IApiReadStorageObjectId[] objectIds)
        {
            try
            {
                return await _progressRepository.LoadAsync(objectIds);
            }
            catch (ApiResponseException ex)
            {
                Print($"[Nakama] Load failed: {ex.Message}", logType: LogType.Error);
                return null;
            }
        }

        public async Task DeleteUserDataAsync(StorageObjectId[] objectIds)
        {
            try
            {
                await _progressRepository.DeleteAsync(objectIds);
                Print("[Nakama] Deleted user data");
            }
            catch (ApiResponseException ex)
            {
                Print($"[Nakama] Deletion failed: {ex.Message}", logType: LogType.Error);
            }
        }
        
        public void SwitchRepository(IUserProgressRepository newRepository)
        {
            _progressRepository = newRepository;
            Print($"[Nakama] Switched to {newRepository.GetType().Name}");
        }

        #endregion

        public void OnDestroy()
        {
            Socket?.CloseAsync();
        }
    }

    public enum LogType
    {
        Log,
        Warning,
        Error
    }
}