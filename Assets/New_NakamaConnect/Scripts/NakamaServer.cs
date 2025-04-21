using System;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using String = ProjectCore.Variables.String;

namespace ProjectCore.SocialFeature.Cloud.Internal
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
            
            Socket.Connected += () =>
            {
                Print("[Nakama] Socket Connected");
            };
            
            Socket.Closed += () =>
            {
                Print("[Nakama] Socket Disconnected");
            };
        }

        #region DeviceIdAuth

        public async Task<ISession> AuthenticateWithDeviceID()
        {
            Print("[Nakama] Authenticating with device ID");
            var deviceID = SocialFeatureConfig.GetDeviceUdid();

            if (Session != null) await KillSession(Session);
            
            try
            {
                var session = await Client.AuthenticateDeviceAsync(deviceID,
                    retryConfiguration: SocialFeatureConfig.GetRetryConfiguration());

                await UpdateSession(session);

                NakamaServerConnected.Invoke();

                Print("[Nakama] Authenticated with device ID: ",deviceID);

                return Session;
            }
            catch (Exception ex)
            {
                Print("[Nakama] Failed to authenticate with device ID: ",ex.Message, LogType.Error);
                return null;
            }
        }

        public async Task LinkWithDeviceID()
        {
            Print("[Nakama] Linking with device ID");
            var deviceID = SocialFeatureConfig.GetDeviceUdid();
            await Client.LinkDeviceAsync(Session, deviceID, SocialFeatureConfig.GetRetryConfiguration());
            Print("[Nakama] Linked with device ID: ",deviceID);
        }

        public async Task UnlinkWithDeviceID(ISession session = null, Func<bool, ApiResponseException, Task> callback = null)
        {
            Print("[Nakama] Unlinking with device ID");
            var deviceID = SocialFeatureConfig.GetDeviceUdid();
            
            session ??= Session;

            try
            {
                await Client.UnlinkDeviceAsync(session, deviceID, SocialFeatureConfig.GetRetryConfiguration());
                Print("[Nakama] Unlinked with device ID: ", deviceID);
                callback?.Invoke(true, null);
            }
            catch (ApiResponseException e)
            {
                Print("[Nakama] Failed to unlink with device ID: ", e.Message);
                callback?.Invoke(false, e);
            }
        }

        #endregion

        #region EmailAuth

        public async Task LinkWithEmail(string email, string password, Func<bool, ApiResponseException, Task> callback = null)
        {
            Print("[Nakama] Syncing with email ID");
            try
            {
                await Client.LinkEmailAsync(Session, email, password, SocialFeatureConfig.GetRetryConfiguration());
                Print("[Nakama] Synced with email ID: " + email);
                callback?.Invoke(true, null);
            }
            catch (ApiResponseException e)
            {
                Print($"[Nakama] Failed to sync with email ID: m:{e.Message} c:{e.GrpcStatusCode}");
                callback?.Invoke(false, e);
            }
        }
        
        public async Task AuthenticateWithEmail(string email, string password, Func<bool, ApiResponseException, ISession, Task> callback = null)
        {
            Print("[Nakama] Authenticating with email ID");
            try
            {
                var session = await Client.AuthenticateEmailAsync(email, password, create: false,
                    retryConfiguration: SocialFeatureConfig.GetRetryConfiguration());

                Print("[Nakama] Authenticated with email ID: " ,email);
                
                callback?.Invoke(true, null, session);
                // NakamaServerConnected.Invoke();
            }
            catch (ApiResponseException ex)
            {
                Print("[Nakama] Failed to authenticate with email ID: " ,ex.Message, LogType.Error);
                callback?.Invoke(false, ex, null);
            }
        }

        public async Task UnlinkWithEmail(string email, string password, Action<bool, ApiResponseException> callback = null)
        {
            Print("[Nakama] Unlinking with email ID");
            try
            {
                await ValidateSession();
                await Client.UnlinkEmailAsync(Session, email, password, SocialFeatureConfig.GetRetryConfiguration());
                Print("[Nakama] Unlinked with email ID: ", email);
                callback?.Invoke(true, null);
            }
            catch (ApiResponseException e)
            {
                Print("[Nakama] Failed to unlink with email ID: " ,e.Message, LogType.Error);
                callback?.Invoke(false, e);
            }
        }

        #endregion

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

                Session = Nakama.Session.Restore(AuthToken, RefreshToken);
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
                    if (forceSync)
                    {
                        Session = await AuthenticateWithDeviceID();
                    }
                    else
                    {
                        Session = await Client.SessionRefreshAsync(Session);
                    }

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
        
        public async Task SaveUserDataAsync(IApiWriteStorageObject[] apiWriteStorageObjects)
        {
            await Client.WriteStorageObjectsAsync(Session, apiWriteStorageObjects);
        }

        public async Task<object> GetUserDataAsync(IApiReadStorageObjectId[] apiReadObjects)
        {
            var result = await Client.ReadStorageObjectsAsync(Session, apiReadObjects);
            return result;
        }

        public async Task DeleteUserDataAsync(StorageObjectId[] apiDeleteObjects)
        {
            await Client.DeleteStorageObjectsAsync(Session, apiDeleteObjects);
            Print("[Nakama] Deleted user data");
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