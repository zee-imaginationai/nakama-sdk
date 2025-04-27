using System.Threading.Tasks;
using Nakama;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama.Internal
{
    [CreateAssetMenu(fileName = "NakamaSessionManager", menuName = "ProjectCore/SocialFeature/Cloud/NakamaSessionManager")]
    [InlineEditor]
    public class NakamaSessionManager : SessionManager<IClient, ISession, ISocket, IApiAccount>
    {
        public override void InitializeManager(IClient client)
        {
            base.InitializeManager(client);
            _Socket = _Client.NewSocket();
            
            _Socket.Connected += () => { Logger.Log("[Nakama] Socket Connected"); };

            _Socket.Closed += () => { Logger.Log("[Nakama] Socket Disconnected"); };
        }

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
                await _Socket.ConnectAsync(Session, SocialFeatureConfig.CanAppearOnline());
                return true;
            }
            catch (ApiResponseException exception)
            {
                Logger.Log($"[Nakama] Socket Connection Failed: {exception.Message}", LogLevel.Error);
                return false;
            }
        }

        public override async Task InitializeSession(IClient client, ISession session)
        {
            await base.InitializeSession(client, session);
            await _Socket.ConnectAsync(session, SocialFeatureConfig.CanAppearOnline(), 10);
            await UpdateAccount();
        }

        public override async Task Disconnect()
        {
            _Socket?.CloseAsync();
        }

        public override async Task<bool> ValidateSession()
        {
            if (Session != null && !Session.IsExpired)
            {
                Logger.Log("[Nakama] Session is Valid!!");
                return true;
            }
            
            Logger.Log("[Nakama] Session is Invalid!!", LogLevel.Error);

            Session = global::Nakama.Session.Restore(AuthToken, RefreshToken);
            if (Session == null)
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
            if (!Session.IsExpired || !Session.IsRefreshExpired)
                return true;

            try
            {
                Session = await _Client.SessionRefreshAsync(Session);

                if (Session != null)
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
            AuthToken.SetValue(Session.AuthToken);
            RefreshToken.SetValue(Session.RefreshToken);
        }

        protected override async Task UpdateAccount()
        {
            try
            {
                Account = await _Client.GetAccountAsync(Session);
                var user = Account.User;
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
                await _Client.DeleteAccountAsync(Session, SocialFeatureConfig.GetRetryConfiguration());
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
                await _Client.SessionLogoutAsync(Session);
            }
            catch (ApiResponseException e)
            {
                Logger.Log($"[Nakama] Failed to kill session: {e.Message}", LogLevel.Error);
            }
        }
    }
}