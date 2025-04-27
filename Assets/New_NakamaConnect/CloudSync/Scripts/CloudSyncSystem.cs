using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using Nakama.TinyJson;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama.Internal
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "CloudSyncSystem", menuName = "ProjectCore/SocialFeature/Cloud/CloudSyncSystem")]
    public class CloudSyncSystem : ScriptableObject
    {
        [SerializeField] private NakamaServer NakamaServer;
        [SerializeField] private UserProfileService UserProfileService;
        
        [SerializeField] private DBBool IsEmailLoggedIn;

        [SerializeField] private DBString Email;
        [SerializeField] private DBString Password;
        [SerializeField] private CloudDBString ConflictString;
        [SerializeField] private MainMenuState MainMenuState;
        
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
        
        #region Helper Functions

        private bool UserHasData(IApiStorageObjects obj)
        {
            if (obj == null) return false;
            var data = obj.Objects.ToList();
            return data.Count != 0;
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