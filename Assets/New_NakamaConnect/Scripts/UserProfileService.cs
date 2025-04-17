using System.Collections.Generic;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.SocialFeature.Cloud.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.SocialFeature
{
    [CreateAssetMenu(fileName = "UserProfileService", menuName = "FPSCommando/SocialFeature/UserProfileService")]
    [InlineEditor]
    public class UserProfileService : ScriptableObject
    {
        [SerializeField] private NakamaServer NakamaServer;
        
        [Button]
        public async Task SaveUserData(string userData)
        {
            Debug.Log("[Nakama] [UserProfileService] Saving User Data");

            if (!await NakamaServer.ValidateSession()) return;
            
            var apiWriteObjects = new IApiWriteStorageObject[]
            {
                new WriteStorageObject
                {
                    Collection = "Save",
                    Key = "UserProgress",
                    Value = userData
                }
            };
            await NakamaServer.SaveUserDataAsync(apiWriteObjects);
            Debug.Log("[Nakama] [UserProfileService] Saved User Data");
        }

        [Button]
        public async Task<object> GetUserData()
        {
            if (!await NakamaServer.ValidateSession()) return null;
            
            var apiReadObjects = new IApiReadStorageObjectId[]
            {
                GetUserProgressObjectId()
            };
            var obj = await NakamaServer.GetUserDataAsync(apiReadObjects);
            return obj;
        }

        [Button]
        public async Task DeleteUserData()
        {
            if (!await NakamaServer.ValidateSession()) return;

            var apiDeleteObjects = new StorageObjectId[]
            {
                GetUserProgressObjectId()
            };
            await NakamaServer.DeleteUserDataAsync(apiDeleteObjects);
        }
        
        private StorageObjectId GetSaveObjectId(string key)
        {
            return new StorageObjectId()
            {
                Collection = "Save",
                Key = key,
                UserId = NakamaServer.Session.UserId
            };
        }

        private StorageObjectId GetUserProgressObjectId()
        {
            return GetSaveObjectId("UserProgress");
        }
    }
}