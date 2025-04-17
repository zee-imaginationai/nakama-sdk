using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
// using Facebook.Unity;
using FPSCommando.SocialFeature.Cloud.Internal;
using Nakama;
using ProjectCore.Events;
using ProjectCore.Variables;
using Nakama.TinyJson;
using ProjectCore.SocialFeature.Cloud.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.SocialFeature.Cloud
{
    [CreateAssetMenu(fileName = "NakamaSystem", menuName = "FPSCommando/SocialFeature/Cloud/NakamaSystem")]
    public class NakamaSystem : ScriptableObject
    {
        [SerializeField] private NakamaServer NakamaServer;
        [SerializeField] private GameEvent NakamaServerConnected;

        [SerializeField] private UserProfileService UserProfileService;

        [SerializeField] private DBInt GameLevel;
        
        [SerializeField] private DBBool IsEmailLoggedIn;

        [SerializeField] private DBString Email;
        [SerializeField] private DBString Password;
        
        private CancellationTokenSource _cancellationTokenSource;

        public async Task Initialize(CancellationToken token)
        {
            NakamaServerConnected.Handler += OnNakamaServerConnected;
            NakamaServer.Initialize();
            
            Debug.LogError("[Nakama System] FB Not Logged In");

            if (!IsEmailLoggedIn)
            {
                await NakamaServer.AuthenticateWithDeviceID(token);
            }
            else
            {
                await NakamaServer.AuthenticateWithEmail(Email, Password, token);
            }
        }

        [Button]
        private async void UnlinkWithEmail()
        {
            await NakamaServer.UnlinkWithEmail(Email, Password);
            await NakamaServer.AuthenticateWithDeviceID(TaskUtil.RefreshToken(ref _cancellationTokenSource));
        }

        [Button]
        private async void UnlinkWithDeviceID()
        {
            await NakamaServer.UnlinkWithDeviceID();
        }

        [Button]
        private async void SyncWithEmail(string email, string password)
        {
            await NakamaServer.SyncWithEmail(email, password, UpdateEmailLogin);
            Email.SetValue(email);
            Password.SetValue(password);
            await SaveUserData();

            return;

            void UpdateEmailLogin(bool status)
            {
                IsEmailLoggedIn.SetValue(status);
            }
        }

        private async void OnNakamaServerConnected()
        {
            // Nakama Server is Connected
            
            // First Read Data in our Desired format. Which is User Progress.
            
            var data = await UserProfileService.GetUserData();
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
                await SaveUserData();
            }
        }

        private async Task SaveUserData()
        {
            var userProgressString = DBManager.GetJsonData();
            await UserProfileService.SaveUserData(userProgressString);
        }
    }
}

namespace FPSCommando.SocialFeature
{
    [CreateAssetMenu(fileName = "UserProfile", menuName = "FPSCommando/SocialFeature/UserProfile")]
    public class UserProfile : ScriptableObject
    {
        public string UserId;
        public string Username;
        public string DisplayName;
        public int AvatarId;

        public bool CanAppearOnline;
    }
}

namespace FPSCommando.SocialFeature
{
}
