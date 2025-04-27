using System;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.CloudService.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama.Internal
{
    [CreateAssetMenu(fileName = "NakamaServer", menuName = "ProjectCore/CloudService/Nakama/NakamaServer")]
    [InlineEditor]
    public class NakamaServer : Server<IClient, ISession, ISocket, IApiAccount>
    {
        public IClient Client { get; private set; }
        
        public void Initialize()
        {
            InitializeSession();
        }

        private void InitializeSession()
        {
            var data = SocialFeatureConfig.GetOriginData();

            Client = new Client(data.Scheme, data.Host, data.Port, data.Key)
            {
                GlobalRetryConfiguration = SocialFeatureConfig.GetRetryConfiguration(),
                Timeout = SocialFeatureConfig.GeneralSettings.RetryTimeOut
            };
            
            SessionManager.InitializeManager(Client);
        }

        #region Authentication_Region

         public override async Task Authenticate(IAuthStrategy<ISession, IClient> strategy, 
            Func<bool, Exception, ISession, Task> callback = null)
        {
            try
            {
                ISession session = await strategy.Authenticate(Client, SocialFeatureConfig);
                await SessionManager.InitializeSession(Client, session);
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

        public override async Task Link(ILinkStrategy<ISession, IClient> strategy, Func<bool, Exception, Task> callback = null)
        {
            Logger.Log($"[Nakama] Unlinking device with {strategy.GetType().Name}");
            try
            {
                await strategy.Link(SessionManager.Session, Client, SocialFeatureConfig);
                callback?.Invoke(true, null);
            }
            catch (ApiResponseException ex)
            {
                Logger.Log($"[Nakama] Failed to link device: {ex.Message}", LogLevel.Error);
                callback?.Invoke(false, ex);
            }
        }

        public override async Task Unlink(IUnlinkStrategy<ISession, IClient> strategy, Func<bool, Exception, Task> callback = null)
        {
            Logger.Log($"[Nakama] Unlinking device with {strategy.GetType().Name}");
            try
            {
                await strategy.Unlink(SessionManager.Session, Client, SocialFeatureConfig);
                callback?.Invoke(true, null);
            }
            catch (ApiResponseException ex)
            {
                Logger.Log($"[Nakama] Failed to unlink device: {ex.Message}", LogLevel.Error);
                callback?.Invoke(false, ex);
            }
        }
        
        #endregion
        
        public void OnDestroy()
        {
            SessionManager.Disconnect();
        }
    }
}