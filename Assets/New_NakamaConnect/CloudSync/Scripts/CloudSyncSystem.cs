using System;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.SocialFeature.Cloud.Internal
{
    [CreateAssetMenu(fileName = "CloudSyncSystem", menuName = "FPSCommando/SocialFeature/Cloud/CloudSyncSystem")]
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

            void Callback(bool success, ApiResponseException exception)
            {
                IsEmailLoggedIn.SetValue(success);
                callback?.Invoke(success, exception);
            }
        }

        public async Task SignOutFromEmail(Action<bool, ApiResponseException> callback = null)
        {
            if (IsConnectedWithMultipleDevices())
            {
                await NakamaServer.UnlinkWithDeviceID(Callback);
                await NakamaServer.AuthenticateWithDeviceID();
            }
            else
            {
                await NakamaServer.UnlinkWithEmail(Email, Password, Callback);
            }
            return;

            void Callback(bool success, ApiResponseException exception)
            {
                IsEmailLoggedIn.SetValue(!success);
                Email.SetValue(string.Empty);
                Password.SetValue(string.Empty);
                callback?.Invoke(success, exception);
            }
        }

        [Button]
        public async Task SigninWithEmail(string email, string password, Action<bool, ApiResponseException> callback = null)
        {
            object data = null;
            if (IsConnectedWithOtherAccounts())
            {
                await NakamaServer.UnlinkWithDeviceID();
            }
            else
            {
                data = await UserProfileService.GetUserData();
                await NakamaServer.DeleteAccount();
            }
            if (data != null) Debug.Log(((IApiStorageObjects) data).Objects?.ToList().FirstOrDefault()?.Value);
            await NakamaServer.AuthenticateWithEmail(email, password, callback);
            await NakamaServer.LinkWithDeviceID();
        }

        private bool IsConnectedWithMultipleDevices()
        {
            var account = NakamaServer.Account;
            var deviceCount = account.Devices.ToList().Count;
            return deviceCount > 1;
        }

        private bool IsConnectedWithOtherAccounts()
        {
            var user = NakamaServer.User;
            var account = NakamaServer.Account;
            return user.AppleId != null || user.GoogleId != null || user.GamecenterId != null || 
                   user.FacebookId != null || user.SteamId != null || user.FacebookInstantGameId != null || account.Email != null;
        }
        
        private async void OnUnlinkEmail(bool success, ApiResponseException exception)
        {
            if (success)
            {
                try
                {
                    await NakamaServer.UnlinkWithDeviceID();
                }
                catch (ApiResponseException e)
                {
                    
                }
            }
            else
            {
                Debug.Log(exception.Message);
            }
        }
        

        #endregion
    }
}