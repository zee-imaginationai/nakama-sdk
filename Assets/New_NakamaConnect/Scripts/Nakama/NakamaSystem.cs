using System;
using System.Threading;
using CustomUtilities.Tools;
using Nakama;
using ProjectCore.CloudService.Internal;
using ProjectCore.Events;
using ProjectCore.Variables;
using ProjectCore.CloudService.Nakama.Internal;
using Sirenix.OdinInspector;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace ProjectCore.CloudService.Nakama
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "NakamaSystem", menuName = "ProjectCore/CloudService/Nakama/NakamaSystem")]
    public class NakamaSystem : ScriptableObject
    {
        [SerializeField] private ServerConfig ServerConfig;
        [SerializeField] private GameEventWithBool FacebookConnectEvent;
        
        [SerializeField] private NakamaUserProgressService UserProgressService;
        [SerializeField] private NakamaCloudSyncService NakamaCloudSyncService;
        [SerializeField] private ServerTimeService ServerTimeService;

        [SerializeField] private Float CloudServiceProgress;

        [SerializeField] private DBString FbAuthToken;
        [SerializeField] private DBBool IsFbSignedIn;
        
        private CancellationTokenSource _cancellationTokenSource;
        private NakamaServer _nakamaServer;
        private CustomLogger _logger;

        public void Initialize()
        {
            FacebookConnectEvent.Handler += OnFacebookConnectEvent;
            CloudServiceProgress.SetValue(0);
            _logger = CreateInstance<CustomLogger>();
            _nakamaServer = new NakamaServer(ServerConfig, _logger, ServerTimeService);
        }
        
        public async Task AuthenticateNakama(CancellationToken token)
        {
            var strategy = IsFbSignedIn
                ? AuthStrategyFactory.CreateFacebookStrategy(FbAuthToken)
                : AuthStrategyFactory.CreateDeviceStrategy();
            await _nakamaServer.Authenticate(strategy, OnAuthCompleted);
        }

        private async void OnFacebookConnectEvent(bool state)
        {
            try
            {
                var strategy = state
                    ? AuthStrategyFactory.CreateFacebookStrategy(FbAuthToken)
                    : AuthStrategyFactory.CreateDeviceStrategy();
                await _nakamaServer.Authenticate(strategy, OnAuthCompleted);
            }
            catch
            {
                _logger.LogError("[Nakama] Failed to authenticate");
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
                await NakamaCloudSyncService.SyncData();
                
                CloudServiceProgress.SetValue(1);
            }
            catch 
            {
                CloudServiceProgress.SetValue(1);
            }
        }
    }
}
