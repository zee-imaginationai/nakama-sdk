using System;
using System.Threading.Tasks;
using CustomUtilities.Tools;
using Nakama;
using ProjectCore.CloudService.Internal;
using Sirenix.OdinInspector;

namespace ProjectCore.CloudService.Nakama.Internal
{
    [InlineEditor]
    public class NakamaServer : Server
    {
        private readonly ServerTimeService _serverTimeService;

        public NakamaServer(ServerConfig config, CustomLogger logger, ServerTimeService serverTimeService) :
            base(config, logger)
        {
            _serverTimeService = serverTimeService;
            var data = _Config.GetNetworkData();

            Client = new Client(data.Scheme, data.Host, data.Port, data.Key)
            {
                GlobalRetryConfiguration = _Config.GetRetryConfiguration(),
                Timeout = _Config.GeneralSettings.RetryTimeOut
            };

            _Socket = Client.NewSocket(true);

            _Socket.Connected += () => { _Logger.Log("[Nakama] Connected to socket", LogLevel.Debug); };
            _Socket.Closed += () => { _Logger.Log("[Nakama] Socket closed", LogLevel.Debug); };
        }

        #region Authentication_Region

        public override async Task Authenticate(IAuthStrategy authStrategy, 
            Func<bool, Exception, Task> callback = null)
        {
            _Logger.Log($"[Nakama] Authenticating with {authStrategy.GetType().Name}");
            try
            {
                ISession session = await authStrategy.Authenticate(Client, _Config);
                Session = session;
                await InitializeSession();
                
                _Logger.Log($"[Nakama] Authenticated with {authStrategy.GetType().Name}");
                
                callback?.Invoke(true, null);
            }
            catch (ApiResponseException ex)
            {
                _Logger.Log($"[Nakama] Failed to authenticate: {ex.Message}", LogLevel.Error);
                callback?.Invoke(false, ex);
            }
            catch (Exception ex)
            {
                _Logger.Log($"[Nakama] Unexpected error: {ex.Message}", LogLevel.Error);
                callback?.Invoke(false, ex);
            }
        }

        public override async Task Link(ILinkStrategy linkStrategy, Func<bool, Exception, Task> callback = null)
        {
            _Logger.Log($"[Nakama] Unlinking device with {linkStrategy.GetType().Name}");
            try
            {
                await linkStrategy.Link(Session, Client, _Config);
                callback?.Invoke(true, null);
            }
            catch (ApiResponseException ex)
            {
                _Logger.Log($"[Nakama] Failed to link device: {ex.Message}", LogLevel.Error);
                callback?.Invoke(false, ex);
            }
        }

        public override async Task Unlink(IUnlinkStrategy unlinkStrategy, Func<bool, Exception, Task> callback = null)
        {
            _Logger.Log($"[Nakama] Unlinking device with {unlinkStrategy.GetType().Name}");
            try
            {
                await unlinkStrategy.Unlink(Session, Client, _Config);
                callback?.Invoke(true, null);
            }
            catch (ApiResponseException ex)
            {
                _Logger.Log($"[Nakama] Failed to unlink device: {ex.Message}", LogLevel.Error);
                callback?.Invoke(false, ex);
            }
        }
        
        #endregion

        #region Session

        protected override async Task<bool> ValidateSocket()
        {
            if (_Socket.IsConnected)
            {
                _Logger.Log("[Nakama] Socket is Valid!!");
                return true;
            }

            _Logger.Log("[Nakama] Socket is Invalid!!", LogLevel.Debug);
            try
            {
                await _Socket.ConnectAsync(Session, _Config.CanAppearOnline());
                return true;
            }
            catch (ApiResponseException exception)
            {
                _Logger.Log($"[Nakama] Socket Connection Failed: {exception.Message}", LogLevel.Error);
                return false;
            }
        }

        private async Task InitializeSession()
        {
            await _Socket.ConnectAsync(Session, _Config.CanAppearOnline(), 10);

            InitializeServices();
            
            await UpdateAccount();
        }

        private void InitializeServices()
        {
            StorageService.Initialize(Client, Session);
            _serverTimeService.Initialize(Client, Session);
        }
        
        public override async Task<bool> ValidateSession()
        {
            if (Session != null && !Session.IsExpired)
            {
                _Logger.Log("[Nakama] Session is Valid!!");
                return true;
            }
            
            _Logger.Log("[Nakama] Session is Invalid!!", LogLevel.Error);

            Session = global::Nakama.Session.Restore(_Config.AuthToken, _Config.RefreshToken);
            if (Session == null)
            {
                _Logger.Log("[Nakama] Failed to restore session", LogLevel.Critical);
                return await RefreshSession();
            }
            var isValidSocket = await ValidateSocket();
            SaveSession();
            return isValidSocket;
        }

        protected override async Task<bool> RefreshSession()
        {
            if (!Session.IsExpired || !Session.IsRefreshExpired)
                return true;

            try
            {
                Session = await Client.SessionRefreshAsync(Session);

                if (Session != null)
                {
                    await ValidateSocket();
                    SaveSession();
                    return true;
                }

                _Logger.Log("[Nakama] Failed to refresh session", LogLevel.Critical);
                return false;
            }
            catch (ApiResponseException ex)
            {
                _Logger.Log($"[Nakama] Failed to validate session: {ex.Message}", LogLevel.Critical);
                return false;
            }
        }
        
        protected override void SaveSession()
        {
            _Config.SetAuthTokens(Session.AuthToken, Session.RefreshToken);
        }

        protected override async Task UpdateAccount()
        {
            try
            {
                _Account = await Client.GetAccountAsync(Session);
                var user = _Account.User;
                _Logger.Log($"[Nakama Account Fetched: {user.Id}");
            }
            catch (ApiResponseException e)
            {
                _Logger.Log($"[Nakama] Failed to get account ID: {e.Message}", LogLevel.Error);
            }
        }

        protected override async Task DeleteAccount()
        {
            try
            {
                await Client.DeleteAccountAsync(Session, _Config.GetRetryConfiguration());
                _Logger.Log("[Nakama] Deleted account]");
            }
            catch (ApiResponseException e)
            {
                _Logger.Log($"[Nakama] Failed to delete account: {e.Message}", LogLevel.Error);
            }
            await KillSession();
        }
        
        private async Task KillSession()
        {
            try
            {
                await Client.SessionLogoutAsync(Session);
            }
            catch (ApiResponseException e)
            {
                _Logger.Log($"[Nakama] Failed to kill session: {e.Message}", LogLevel.Error);
            }
        }

        #endregion
        
        public void OnDestroy()
        {
            _Socket?.CloseAsync();
        }
    }
}