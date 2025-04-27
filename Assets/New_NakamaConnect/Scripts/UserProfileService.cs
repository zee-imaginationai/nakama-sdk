using System.Threading.Tasks;
using Nakama;
using ProjectCore.CloudService.Nakama.Internal;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama
{
    [CreateAssetMenu(fileName = "UserProfileService", menuName = "ProjectCore/SocialFeature/UserProfileService")]
    [InlineEditor]
    public class UserProfileService : ScriptableObject
    {
        [SerializeField] private NakamaStorageService NakamaStorageService;

        public UserProfileService(NakamaStorageService nakamaStorageService)
        {
            NakamaStorageService = nakamaStorageService;
        }

        [Button]
        public async Task SaveUserData()
        {
            var data = DBManager.GetJsonData();
            await SaveUserData(data);
        }
        
        public async Task SaveUserData(string userData)
        {
            Debug.Log("[Nakama] [UserProfileService] Saving User Data");

            if (!await NakamaStorageService.ValidateSession()) return;
            
            var apiWriteObjects = new IApiWriteStorageObject[]
            {
                new WriteStorageObject
                {
                    Collection = "Save",
                    Key = "UserProgress",
                    Value = userData
                }
            };
            await NakamaStorageService.SaveUserDataAsync(apiWriteObjects);
            Debug.Log("[Nakama] [UserProfileService] Saved User Data");
        }

        [Button]
        public async Task<IApiStorageObjects> GetUserData()
        {
            if (!await NakamaStorageService.ValidateSession()) return null;
            
            var apiReadObjects = new IApiReadStorageObjectId[]
            {
                GetUserProgressObjectId()
            };
            var obj = await NakamaStorageService.GetUserDataAsync(apiReadObjects);
            return obj;
        }

        [Button]
        public async Task DeleteUserData()
        {
            if (!await NakamaStorageService.ValidateSession()) return;

            var apiDeleteObjects = new StorageObjectId[]
            {
                GetUserProgressObjectId()
            };
            await NakamaStorageService.DeleteUserDataAsync(apiDeleteObjects);
        }
        
        private StorageObjectId GetSaveObjectId(string key)
        {
            return new StorageObjectId()
            {
                Collection = "Save",
                Key = key,
                UserId = NakamaStorageService.Session.UserId
            };
        }

        private StorageObjectId GetUserProgressObjectId()
        {
            return GetSaveObjectId("UserProgress");
        }
    }
}