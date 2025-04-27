using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.Events;
using ProjectCore.Variables;
using Nakama.TinyJson;
using ProjectCore.CloudService.Nakama.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "NakamaSystem", menuName = "ProjectCore/SocialFeature/Cloud/NakamaSystem")]
    public class NakamaSystem : ScriptableObject
    {
        [SerializeField] private NakamaServer NakamaServer;
        [SerializeField] private GameEvent NakamaServerConnected;
        
        [SerializeField] private UserProfileService UserProfileService;

        [SerializeField] private Float CloudServiceProgress;

        [SerializeField] private DBString FbAuthToken;
        
        [SerializeField] private DBInt GameLevel;
        [SerializeField] private DBBool IsFbSignedIn;
        
        private CancellationTokenSource _cancellationTokenSource;

        public void Initialize()
        {
            NakamaServerConnected.Handler += OnNakamaServerConnected;
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

        private async void OnNakamaServerConnected()
        {
            // Nakama Server is Connected
            
            // First Read Data in our Desired format. Which is User Progress.
            
            var data = await UserProfileService.GetUserData();

            if (data == null)
            {
                Debug.LogError("[Nakama System] No Data Received");
                return;
            }
            
            var storageObjects = (IApiStorageObjects) data;
            
            // Check if there is any data received in the request.

            var dataObject = storageObjects.Objects.ToList();
            
            if (dataObject.Count > 0)
            {
                // Here some data is received from the server.

                var userProgressString = storageObjects.Objects.First().Value;
                
                Debug.LogError($"[Nakama] UserProgress : {userProgressString}");

                // Now check if the data received in the request is latest or older than the local data.

                var progressJson = userProgressString.FromJson<Dictionary<string, object>>();
                
                // here update the JSON in DBManager
                
                DBManager.LoadJsonData(progressJson);
                
                Debug.LogError("[Nakama] Loaded User Profile");
            }
            else
            {
                // Here no data is received from the server.
                // Update data on server from Local storage.
                await UserProfileService.SaveUserData();
            }
            
            CloudServiceProgress.SetValue(1);
        }
    }
}
