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
using String = ProjectCore.Variables.String;

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

        [SerializeField] private String GoogleAccessToken;
        [SerializeField] private Bool GPGSLoggedIn;
        
        [SerializeField] private GameEventWithBool GPGSConnectEvent;
        
        private Server _nakamaServer;

        public void Initialize()
        {
#if FB
            FacebookConnectEvent.Handler += OnFacebookConnectEvent;
#endif

            GPGSConnectEvent.Handler += OnGPGSConnectEvent;
            
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

        private async void OnGPGSConnectEvent(bool state)
        {
            if (!state)
            {
                try
                {
                    _nakamaServer.ClearSession();
                    await ((Internal.NakamaServer)_nakamaServer).KillSession();
                    return;
                }
                catch
                {
                    Logger.LogError("[Nakama] Failed to logout");
                    return;
                }
            }
            try
            {
                var strategy = AuthStrategyFactory.CreateGoogleStrategy(GoogleAccessToken);
                await _nakamaServer.Authenticate(strategy, callback: OnAuthCompleted);
            }
            catch
            {
                Logger.LogError("[Nakama] Failed to authenticate");
            }
        }

#if FB
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
#endif

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
