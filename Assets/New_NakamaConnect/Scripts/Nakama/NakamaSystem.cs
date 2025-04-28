using System;
using System.Threading;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.Events;
using ProjectCore.Variables;
using ProjectCore.CloudService.Nakama.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "NakamaSystem", menuName = "ProjectCore/CloudService/Nakama/NakamaSystem")]
    public class NakamaSystem : ScriptableObject
    {
        [SerializeField] private NakamaServer NakamaServer;
        [SerializeField] private GameEvent NakamaServerConnected;
        [SerializeField] private GameEventWithBool FacebookConnectEvent;
        
        [SerializeField] private NakamaUserProgressService UserProgressService;
        [SerializeField] private NakamaCloudSyncService NakamaCloudSyncService;

        [SerializeField] private Float CloudServiceProgress;

        [SerializeField] private DBString FbAuthToken;
        
        [SerializeField] private DBInt GameLevel;
        [SerializeField] private DBBool IsFbSignedIn;
        
        private CancellationTokenSource _cancellationTokenSource;

        public void Initialize()
        {
            NakamaServerConnected.Handler += OnNakamaServerConnected;
            FacebookConnectEvent.Handler += OnFacebookConnectEvent;
            NakamaServer.Initialize();
            CloudServiceProgress.SetValue(0);
        }
        
        public async Task AuthenticateNakama(CancellationToken token)
        {
            var strategy = IsFbSignedIn
                ? AuthStrategyFactory.CreateFacebookStrategy(FbAuthToken)
                : AuthStrategyFactory.CreateDeviceStrategy();
            await NakamaServer.Authenticate(strategy);
        }

        private async void OnFacebookConnectEvent(bool state)
        {
            try
            {
                var strategy = state
                    ? AuthStrategyFactory.CreateFacebookStrategy(FbAuthToken)
                    : AuthStrategyFactory.CreateDeviceStrategy();
                await NakamaServer.Authenticate(strategy, OnAuthCompleted);
                return;

                async Task OnAuthCompleted(bool success, Exception exception, ISession session)
                {
                    if (success)
                    {
                        await NakamaCloudSyncService.SyncData();
                    }
                }
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private async void OnNakamaServerConnected()
        {
            try
            {
                // Nakama Server is Connected
            
                // First Read Data in our Desired format. Which is User Progress.
            
                var data = await UserProgressService.GetUserData();

                // Check if there is any data received in the request.
            
                if (data == null)
                {
                    // Here no data is received from the server.
                    Debug.LogError("[Nakama System] No Data Received");
                
                    // Update data on server from Local storage.
                    await UserProgressService.SaveUserData();
                }
                else
                {
                    // Here some data is received from the server.
                    Debug.LogError($"[Nakama] UserProgress : {data}");
                
                    // Now check if the data received in the request is latest or older than the local data.
                
                    // here update the JSON in DBManager
                    DBManager.LoadJsonData(data);
                    Debug.LogError("[Nakama] Loaded User Profile");
                }
                CloudServiceProgress.SetValue(1);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}
