using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using Nakama.TinyJson;
using ProjectCore.Events;
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
        [SerializeField] private FacebookService FacebookService;
        
        [SerializeField] private DBBool IsEmailLoggedIn;

        [SerializeField] private DBString Email;
        [SerializeField] private DBString Password;
        [SerializeField] private CloudDBString ConflictString;
        [SerializeField] private MainMenuState MainMenuState;

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
                await OnSyncComplete();
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
                
                await NakamaServer.UpdateSession(session);
                await NakamaServer.LinkWithDeviceID();
                
                object data = await UserProfileService.GetUserData();
                if (data != null) 
                    Debug.Log(((IApiStorageObjects) data).Objects?.ToList().FirstOrDefault()?.Value);

                await OnSyncComplete(data);
            }
        }

        private async Task OnSyncComplete(object data = null)
        {
            var conflictData = await ResolveConflict(data);
            
            switch (conflictData.StorageType)
            {
                case StorageType.Cloud:
                    DBManager.LoadJsonData(conflictData.Data);
                    break;
                case StorageType.Local:
                case StorageType.None:
                default:
                    await UserProfileService.SaveUserData();
                    break;
            }
            MainMenuState.UpdateButton();
        }

        private async Task<ConflictData> ResolveConflict(object data = null)
        {
            if (data == null) return new ConflictData()
            {
                StorageType = StorageType.Local,
                Data = null
            };
            
            var progress = data as IApiStorageObjects;
            var apiStorageObjects = progress?.Objects?.ToList().FirstOrDefault()?.Value;
            var userProgress = apiStorageObjects.FromJson<Dictionary<string, object>>();
            var conflictKey = ConflictString.GetKey();
            if (userProgress[conflictKey] != null && (string)userProgress[conflictKey] != ConflictString.GetValue())
            {
                Debug.LogError("[CloudSync] Conflict found\n" +
                               "User Progress: " + userProgress[conflictKey] + "\n" +
                               "Local Progress: " + ConflictString.GetValue());
                return new ConflictData(){
                    StorageType = await MainMenuState.ResolveConflict(),
                    Data = userProgress
                };
            }
            Debug.LogError("[CloudSync] No Conflict Found");
            return new ConflictData()
            {
                StorageType = StorageType.Local,
                Data = null
            };
        }

        [Button]
        private bool IsConnectedWithMultipleDevices()
        {
            var account = NakamaServer.Account;
            var deviceCount = account.Devices.ToList().Count;
            Debug.LogError("[CloudSync] Account: " + account.User.Id + " Device Count: " + deviceCount);
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
        
        #region FacebookSyncRegion

        [SerializeField] private DBString FbAccessToken;
        [SerializeField] private GameEventWithBool FacebookConnectEvent;
        
        [Button]
        public async Task LinkWithFacebook(Action<bool, ApiResponseException> callback = null)
        {
            await FacebookService.LoginFacebook();
            await NakamaServer.LinkWithFacebook(FbAccessToken.GetValue(), Callback);
            return;
            
            Task Callback(bool success, ApiResponseException exception)
            {
                callback?.Invoke(success, exception);
                if (!success) return Task.CompletedTask;
                return OnSyncComplete();
            }
        }
        
        [Button]
        public async Task UnlinkWithFacebook(Action<bool, ApiResponseException> callback = null)
        {
            await FacebookService.LoginFacebook();
            await NakamaServer.UnlinkWithFacebook(FbAccessToken.GetValue(), Callback);
            return;
            
            Task Callback(bool success, ApiResponseException exception)
            {
                callback?.Invoke(success, exception);
                if (!success) return Task.CompletedTask;
                
                FacebookService.LogoutFacebook();
                return OnSyncComplete();
            }
        }
        
        [Button]
        public async Task SigninWithFacebook(Action<bool, ApiResponseException> callback = null)
        {
            FacebookService.LoginFacebook();
            FacebookConnectEvent.Handler += OnFacebookConnectEvent;
            
            return;
            
            async Task Callback(bool success, ApiResponseException exception, ISession session)
            {
                callback?.Invoke(success, exception);
                if (!success) return;
                
                await NakamaServer.UpdateSession(session);
                await OnFbSyncComplete();
            }
        }

        private async void OnFacebookConnectEvent(bool isConnected)
        {
            FacebookConnectEvent.Handler -= OnFacebookConnectEvent;

            try
            {
                await NakamaServer.AuthenticateWithFacebook(FbAccessToken.GetValue(), Callback);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return;
            
            async Task Callback(bool success, ApiResponseException exception, ISession session)
            {
                // callback?.Invoke(success, exception);
                if (!success) return;
                
                await NakamaServer.UpdateSession(session);
                await OnFbSyncComplete();
            }
        }

        private async Task SigninFacebook()
        {
            
        }

        private async Task OnFbSyncComplete()
        {
            
        }
        
        #endregion
    }

    public struct ConflictData
    {
        public StorageType StorageType;
        public Dictionary<string, object> Data;
    }
    
    public enum StorageType
    {
        None,
        Local,
        Cloud
    }
}