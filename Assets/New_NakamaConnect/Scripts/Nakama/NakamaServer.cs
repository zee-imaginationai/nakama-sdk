using System;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.CloudService.Internal;
using Sirenix.OdinInspector;
using UnityEngine;
using IClient = Nakama.IClient;

namespace ProjectCore.CloudService.Nakama.Internal
{
    [CreateAssetMenu(fileName = "NakamaServer", menuName = "ProjectCore/CloudService/Nakama/NakamaServer")]
    [InlineEditor]
    public class NakamaServer : Server
    {
        public override void Initialize()
        {
            base.Initialize();
            var data = Config.GetOriginData();

            _Client = new Client(data.Scheme, data.Host, data.Port, data.Key)
            {
                GlobalRetryConfiguration = Config.GetRetryConfiguration(),
                Timeout = Config.GeneralSettings.RetryTimeOut
            };

            _Socket = _Client.NewSocket(true);

            _Socket.Connected += () => { Logger.Log("[Nakama] Connected to socket", LogLevel.Debug); };
            _Socket.Closed += () => { Logger.Log("[Nakama] Socket closed", LogLevel.Debug); };
        }

        #region Authentication_Region

         public override async Task Authenticate(IAuthStrategy strategy, 
            Func<bool, Exception, ISession, Task> callback = null)
        {
            try
            {
                ISession session = await strategy.Authenticate(_Client, Config);
                _Session = session;
                await InitializeSession();
                
                Logger.Log($"[Nakama] Authenticated with {strategy.GetType().Name}");
                ServerConnectedEvent.Invoke();
                callback?.Invoke(true, null, session);
            }
            catch (ApiResponseException ex)
            {
                Logger.Log($"[Nakama] Failed to authenticate: {ex.Message}", LogLevel.Error);
                callback?.Invoke(false, ex, null);
            }
            catch (Exception ex)
            {
                Logger.Log($"[Nakama] Unexpected error: {ex.Message}", LogLevel.Error);
                callback?.Invoke(false, null, null);
            }
        }

        public override async Task Link(ILinkStrategy strategy, Func<bool, Exception, Task> callback = null)
        {
            Logger.Log($"[Nakama] Unlinking device with {strategy.GetType().Name}");
            try
            {
                await strategy.Link(_Session, _Client, Config);
                callback?.Invoke(true, null);
            }
            catch (ApiResponseException ex)
            {
                Logger.Log($"[Nakama] Failed to link device: {ex.Message}", LogLevel.Error);
                callback?.Invoke(false, ex);
            }
        }

        public override async Task Unlink(IUnlinkStrategy strategy, Func<bool, Exception, Task> callback = null)
        {
            Logger.Log($"[Nakama] Unlinking device with {strategy.GetType().Name}");
            try
            {
                await strategy.Unlink(_Session, _Client, Config);
                callback?.Invoke(true, null);
            }
            catch (ApiResponseException ex)
            {
                Logger.Log($"[Nakama] Failed to unlink device: {ex.Message}", LogLevel.Error);
                callback?.Invoke(false, ex);
            }
        }
        
        #endregion

        #region Session

        protected override async Task<bool> ValidateSocket()
        {
            if (_Socket.IsConnected)
            {
                Logger.Log("[Nakama] Socket is Valid!!");
                return true;
            }

            Logger.Log("[Nakama] Socket is Invalid!!", LogLevel.Debug);
            try
            {
                await _Socket.ConnectAsync(_Session, Config.CanAppearOnline());
                return true;
            }
            catch (ApiResponseException exception)
            {
                Logger.Log($"[Nakama] Socket Connection Failed: {exception.Message}", LogLevel.Error);
                return false;
            }
        }

        private async Task InitializeSession()
        {
            await _Socket.ConnectAsync(_Session, Config.CanAppearOnline(), 10);
            StorageService.Initialize(_Client, _Session);
            await UpdateAccount();
        }
        
        public override async Task<bool> ValidateSession()
        {
            if (_Session != null && !_Session.IsExpired)
            {
                Logger.Log("[Nakama] Session is Valid!!");
                return true;
            }
            
            Logger.Log("[Nakama] Session is Invalid!!", LogLevel.Error);

            _Session = Session.Restore(AuthToken, RefreshToken);
            if (_Session == null)
            {
                Logger.Log("[Nakama] Failed to restore session", LogLevel.Critical);
                return await RefreshSession();
            }
            var isValidSocket = await ValidateSocket();
            SaveSession();
            return isValidSocket;
        }

        protected override async Task<bool> RefreshSession()
        {
            if (!_Session.IsExpired || !_Session.IsRefreshExpired)
                return true;

            try
            {
                _Session = await _Client.SessionRefreshAsync(_Session);

                if (_Session != null)
                {
                    await ValidateSocket();
                    SaveSession();
                    return true;
                }

                Logger.Log("[Nakama] Failed to refresh session", LogLevel.Critical);
                return false;
            }
            catch (ApiResponseException ex)
            {
                Logger.Log($"[Nakama] Failed to validate session: {ex.Message}", LogLevel.Critical);
                return false;
            }
        }
        
        protected override void SaveSession()
        {
            AuthToken.SetValue(_Session.AuthToken);
            RefreshToken.SetValue(_Session.RefreshToken);
        }

        protected override async Task UpdateAccount()
        {
            try
            {
                _Account = await _Client.GetAccountAsync(_Session);
                var user = _Account.User;
                Logger.Log($"[Nakama Account Fetched: {user.Id}");
            }
            catch (ApiResponseException e)
            {
                Logger.Log($"[Nakama] Failed to get account ID: {e.Message}", LogLevel.Error);
            }
        }

        protected override async Task DeleteAccount()
        {
            try
            {
                await _Client.DeleteAccountAsync(_Session, Config.GetRetryConfiguration());
                Logger.Log("[Nakama] Deleted account]");
            }
            catch (ApiResponseException e)
            {
                Logger.Log($"[Nakama] Failed to delete account: {e.Message}", LogLevel.Error);
            }
            await KillSession();
        }
        
        private async Task KillSession()
        {
            try
            {
                await _Client.SessionLogoutAsync(_Session);
            }
            catch (ApiResponseException e)
            {
                Logger.Log($"[Nakama] Failed to kill session: {e.Message}", LogLevel.Error);
            }
        }

        #endregion
        
        public void OnDestroy()
        {
            _Socket?.CloseAsync();
        }
    }
}