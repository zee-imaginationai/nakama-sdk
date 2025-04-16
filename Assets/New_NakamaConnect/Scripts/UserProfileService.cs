using System.Collections.Generic;
using System.Threading.Tasks;
using FPSCommando.SocialFeature.Cloud.Internal;
using Nakama;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSCommando.SocialFeature
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
            List<IApiWriteStorageObject> apiWriteObjects = new List<IApiWriteStorageObject>
            {
                new WriteStorageObject
                {
                    Collection = "Save",
                    Key = "UserProgress",
                    Value = userData
                }
            };

            await NakamaServer.SaveUserDataAsync(apiWriteObjects);
        }

        [Button]
        public async Task<object> GetUserData()
        {
            var obj = await NakamaServer.GetUserDataAsync();
            return obj;
        }

        public async Task DeleteUserData()
        {
            await NakamaServer.DeleteUserDataAsync();
        }
    }
}