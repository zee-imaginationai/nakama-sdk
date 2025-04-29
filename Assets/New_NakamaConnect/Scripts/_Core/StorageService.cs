using System.Collections.Generic;
using System.Threading.Tasks;
using CustomUtilities.Tools;
using Nakama;
using UnityEngine;

namespace ProjectCore.CloudService.Internal
{
    public abstract class StorageService : ScriptableObject
    {
        [SerializeField] private CustomLogger Logger;
        
        protected IClient _Client;
        protected ISession _Session;
        
        protected CloudStorageService _CloudStorageService;

        public void Initialize(IClient client, ISession session)
        {
            _Client = client;
            _Session = session;
            CreateCloudStorageService();
        }

        protected abstract void CreateCloudStorageService();
        
        public async Task SaveData(string collection, string key, string data)
        {
            Logger.Log($"Saving {collection} to {key}]");
            await _CloudStorageService.Save(collection, key, data);
            Logger.Log($"Saved {collection} to {key}");
        }

        public async Task<Dictionary<string, object>> LoadData(string collection, string key)
        {
            Logger.Log($"Loading {collection} from {key}");
            return await _CloudStorageService.Load<Dictionary<string, object>>(collection, key);
        }

        public async Task DeleteData(string collection, string key)
        {
            Logger.Log($"Deleting {collection} from {key}");
            await _CloudStorageService.Delete(collection, key);
            Logger.Log($"Deleted {collection}");
        }
    }
}