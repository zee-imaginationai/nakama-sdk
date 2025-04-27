using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.CloudService.Internal
{
    public abstract class UserProgressService<TClient, TSession> : ScriptableObject
    {
        [SerializeField] private StorageService<TClient, TSession> StorageService;
        
        private const string COLLECTION_NAME = "Save";
        private const string KEY_NAME = "UserProgress";

        [Button]
        public async Task SaveUserData()
        {
            Debug.Log("[UserProfileService] Saving User Data");
            
            await StorageService.SaveData(COLLECTION_NAME, KEY_NAME, DBManager.GetJsonData());
            
            Debug.Log("[UserProfileService] Saved User Data");
        }

        [Button]
        public async Task<Dictionary<string, object>> GetUserData()
        {
            var obj = await StorageService.LoadData(COLLECTION_NAME, KEY_NAME);
            return obj;
        }

        [Button]
        public async Task DeleteUserData()
        {
            await StorageService.DeleteData(COLLECTION_NAME, KEY_NAME);
        }
    }
}