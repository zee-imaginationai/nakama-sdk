using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
// using Facebook.Unity;
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
        public async void UnlinkWithEmail()
        {
            var task = NakamaServer.UnlinkWithEmail(Email, Password, OnUnlinkEmail);
            await task;
            if (task.IsFaulted) return;
            
            Email.SetValue(string.Empty);
            Password.SetValue(string.Empty);
            
            await NakamaServer.AuthenticateWithDeviceID(TaskUtil.RefreshToken(ref _cancellationTokenSource));
            
            return;
            
            void OnUnlinkEmail(bool status)
            {
                IsEmailLoggedIn.SetValue(!status);
            }
        }

        [Button]
        private async void AuthenticateWithEmail()
        {
            await NakamaServer.AuthenticateWithEmail(Email, Password, TaskUtil.RefreshToken(ref _cancellationTokenSource));
        }

        [Button]
        private async void UnlinkWithDeviceID()
        {
            await NakamaServer.UnlinkWithDeviceID();
        }

        [Button]
        public async Task<string> SyncWithEmail(string email, string password)
        {
            var status = await NakamaServer.LinkWithEmail(email, password, UpdateEmailLogin);

            if (!IsEmailLoggedIn)
            {
                return status;
            }
            
            Email.SetValue(email);
            Password.SetValue(password);
            
            await SaveUserData();

            return status;

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

namespace ProjectCore.SocialFeature
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

namespace ProjectCore.SocialFeature
{
}
