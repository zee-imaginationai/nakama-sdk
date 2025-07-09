using System;
using System.Threading;
using System.Threading.Tasks;
using CustomUtilities.Tools;
using ExtensionMethods;
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
#if GPGS
        [SerializeField] private String GoogleAccessToken;
        [SerializeField] private Bool GPGSLoggedIn;
        
        [SerializeField] private GameEventWithBool GPGSConnectEvent;
#endif

        private Server _nakamaServer;

        public void Initialize()
        {
            CloudServiceProgress.SetValue(0);
            _nakamaServer = new Internal.NakamaServer(ServerConfig, Logger);
#if FB
            FacebookConnectEvent.Handler += OnFacebookConnectEvent;
            Logger.Log("[Nakama] Facebook Authentication Method selected!");
#endif
#if GPGS
            GPGSConnectEvent.Handler += OnGPGSConnectEvent;
            Logger.Log("[Nakama] Google Authentication Method selected!");
#endif
#if !FB && !GPGS
            Logger.LogError("[Nakama] No authentication method selected!");
            CloudServiceProgress.SetValue(1);
#elif FB && GPGS
            Logger.Log("[Nakama] FB & Google Authentication Method selected!");
#endif
        }

        public async Task AuthenticateNakama(CancellationToken token)
        {
#if FB
            if (IsFbSignedIn)
            {
                var strategy = AuthStrategyFactory.CreateFacebookStrategy(FbAuthToken);
                await _nakamaServer.Authenticate(strategy, token, OnAuthCompleted);
            }
#endif
#if GPGS
            if(GPGSLoggedIn)
            {
                await AuthNakamaWithGPGS(token);
            }
#endif
#if !FB && !GPGS
            await Task.CompletedTask;
#endif
        }

#if GPGS
        private async void OnGPGSConnectEvent(bool state)
        {
            if (!state)
            {
                DisconnectServer();
                return;
            }

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            var token = tokenSource.RefreshToken();
            await AuthNakamaWithGPGS(token);
        }

        private async Task AuthNakamaWithGPGS(CancellationToken token)
        {
            try
            {
                var strategy = AuthStrategyFactory.CreateGoogleStrategy(GoogleAccessToken);
                await _nakamaServer.Authenticate(strategy, token, OnAuthCompleted);
            }
            catch
            {
                Logger.LogError("[Nakama] Failed to authenticate");
            }
        }
#endif

        private async void DisconnectServer()
        {
            try
            {
                _nakamaServer.ClearSession();
                await ((Internal.NakamaServer)_nakamaServer).KillSession();
            }
            catch
            {
                Logger.LogError("[Nakama] Failed to logout");
            }
        }

#if FB
        private async void OnFacebookConnectEvent(bool state)
        {
            if (!state)
            {
                DisconnectServer();
                return;
            }

            await AuthNakamaWithFacebook();
        }
        
        private async Task AuthNakamaWithFacebook()
        { 
            try
            {
                var strategy = AuthStrategyFactory.CreateFacebookStrategy(FbAuthToken);
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
