using System;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.SocialFeature.Cloud.Internal
{
    [CreateAssetMenu(fileName = "CloudSyncSystem", menuName = "ProjectCore/SocialFeature/Cloud/CloudSyncSystem")]
    public class CloudSyncSystem : ScriptableObject
    {
        [SerializeField] private NakamaServer NakamaServer;
        [SerializeField] private UserProfileService UserProfileService;
        
        [SerializeField] private DBBool IsEmailLoggedIn;

        [SerializeField] private DBString Email;
        [SerializeField] private DBString Password;

        #region EmailSyncRegion
        
        [Button]
        public async Task SignupWithEmail(string email, string password, Action<bool, ApiResponseException> callback = null)
        {
            await NakamaServer.LinkWithEmail(email, password, Callback);
            return;

            async Task Callback(bool success, ApiResponseException exception)
            {
                callback?.Invoke(success, exception);
                var signedIn = OnSignIn(success, email, password);
                if (!signedIn) return;
                await UserProfileService.SaveUserData();
            }
        }

        public async Task SignOutFromEmail(Action<bool, ApiResponseException> callback = null)
        {
            if (IsConnectedWithMultipleDevices())
            {
                await NakamaServer.UnlinkWithDeviceID(callback: UnlinkWithDeviceCallback);
            }
            else
            {
                await NakamaServer.UnlinkWithEmail(Email, Password, Callback);
            }
            return;

            async Task UnlinkWithDeviceCallback(bool success, ApiResponseException exception)
            {
                Callback(success, exception);
                if (!success) return;
                
                await NakamaServer.AuthenticateWithDeviceID();
                await UserProfileService.SaveUserData();
            }
            
            void Callback(bool success, ApiResponseException exception)
            {
                callback?.Invoke(success, exception);
                IsEmailLoggedIn.SetValue(!success);
                if (!success) return;
                
                Email.SetValue(string.Empty);
                Password.SetValue(string.Empty);
            }
        }
        
        [Button]
        public async Task SigninWithEmail(string email, string password, Action<bool, ApiResponseException> callback = null)
        {
            object data = await UserProfileService.GetUserData();
            
            await NakamaServer.AuthenticateWithEmail(email, password, Callback);
            return;

            async Task Callback(bool success, ApiResponseException exception, ISession session)
            {
                callback?.Invoke(success, exception);
                var signedIn = OnSignIn(success, email, password);
                if (!signedIn) return;
                
                if (IsConnectedWithOtherAccounts())
                {
                    await NakamaServer.UnlinkWithDeviceID();
                }
                else
                {
                    await NakamaServer.DeleteAccount();
                }
                if (data != null) 
                    Debug.Log(((IApiStorageObjects) data).Objects?.ToList().FirstOrDefault()?.Value);
                
                await NakamaServer.UpdateSession(session);
                await NakamaServer.LinkWithDeviceID();
                await OnSyncComplete();
            }
        }

        private async Task OnSyncComplete()
        {
            await UserProfileService.SaveUserData();
        }

        private bool IsConnectedWithMultipleDevices()
        {
            var account = NakamaServer.Account;
            var deviceCount = account.Devices.ToList().Count;
            Debug.LogError("[CloudSync] Device Count: " + deviceCount);
            return deviceCount > 1;
        }

        private bool IsConnectedWithOtherAccounts()
        {
            var user = NakamaServer.User;
            var account = NakamaServer.Account;
            return user.AppleId != null || user.GoogleId != null || user.GamecenterId != null || 
                   user.FacebookId != null || user.SteamId != null || user.FacebookInstantGameId != null || account.Email != null;
        }

        private bool OnSignIn(bool success, string email, string password)
        {
            IsEmailLoggedIn.SetValue(success);
            if (!success) return false;
            Email.SetValue(email);
            Password.SetValue(password);
            return true;
        }
        
        #endregion
    }
}