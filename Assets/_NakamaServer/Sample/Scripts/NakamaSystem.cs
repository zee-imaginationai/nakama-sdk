using System;
using System.Threading;
using System.Threading.Tasks;
using CustomUtilities.Tools;
using ProjectCore.Events;
using ProjectCore.Integrations.Internal;
using ProjectCore.Integrations.NakamaServer.Internal;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.Integrations.NakamaServer
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "NakamaSystem", menuName = "ProjectCore/Integrations/NakamaServer/NakamaSystem")]
    public class NakamaSystem : ScriptableObject
    {
        [SerializeField] private ServerConfig ServerConfig;
        
        [SerializeField] private NakamaStorageService NakamaStorageService;
        [SerializeField] private NakamaCloudSyncService NakamaCloudSyncService;
        [SerializeField] private ServerTimeService ServerTimeService;
        [SerializeField] private CustomLogger Logger;

        [SerializeField] private Float CloudServiceProgress;

#if FB
        [SerializeField] private DBString FbAuthToken;
        [SerializeField] private DBBool IsFbSignedIn;
        
        [SerializeField] private GameEventWithBool FacebookConnectEvent;
#endif
        
        private Server _nakamaServer;

        public void Initialize()
        {
#if FB
            FacebookConnectEvent.Handler += OnFacebookConnectEvent;
#endif
            NakamaCloudSyncService.Initialize();
            CloudServiceProgress.SetValue(0);
            _nakamaServer = new Internal.NakamaServer(ServerConfig, Logger);
        }
        
        public async Task AuthenticateNakama(CancellationToken token)
        {
#if FB
            var strategy = IsFbSignedIn
                ? AuthStrategyFactory.CreateFacebookStrategy(FbAuthToken)
                : AuthStrategyFactory.CreateDeviceStrategy();
#else
            var strategy = AuthStrategyFactory.CreateDeviceStrategy();
#endif
            await _nakamaServer.Authenticate(strategy, token, OnAuthCompleted);
        }

        private async void OnFacebookConnectEvent(bool state)
        {
            try
            {
                var strategy = state
                    ? AuthStrategyFactory.CreateFacebookStrategy(FbAuthToken)
                    : AuthStrategyFactory.CreateDeviceStrategy();
                await _nakamaServer.Authenticate(strategy, callback: OnAuthCompleted);
            }
            catch
            {
                Logger.LogError("[Nakama] Failed to authenticate");
            }
        }

        private async Task OnAuthCompleted(bool success, Exception exception)
        {
            if (!success)
            {
                CloudServiceProgress.SetValue(1);
                return;
            }
            try
            {
                // Sync Data
                
                InitializeServices();
                
                await NakamaCloudSyncService.SyncData();
                
                CloudServiceProgress.SetValue(1);
            }
            catch 
            {
                CloudServiceProgress.SetValue(1);
            }
        }
        
        private void InitializeServices()
        {
            NakamaStorageService.Initialize(_nakamaServer.Client, _nakamaServer.Session, Logger);
            ServerTimeService.Initialize(_nakamaServer.Client, _nakamaServer.Session, Logger);
        }

        private void OnDestroy()
        {
            _nakamaServer.ClearSession();
        }
    }
}
