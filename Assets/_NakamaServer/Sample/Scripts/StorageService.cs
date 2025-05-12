using System.Collections.Generic;
using System.Threading.Tasks;
using CustomUtilities.Tools;
using Nakama;
using UnityEngine;

namespace ProjectCore.Integrations.Internal
{
    public abstract class StorageService : ScriptableObject
    {
        protected IStorageProvider _Provider;
        protected ISerializationStrategy _Serializer;
        protected CustomLogger _Logger;
        
        public abstract void Initialize(IClient client, ISession session, CustomLogger logger);
        
        public abstract void SaveUserProgress();
        
        public abstract Task<Dictionary<string, object>> LoadUserProgress();
        
        public abstract Task DeleteUserProgress();
        
        protected async Task SaveData(string collection, string key, string data)
        {
            _Logger.Log($"Saving {collection} to {key}]");
            await _Provider.SaveDataAsync(collection, key, data);
            _Logger.Log($"Saved {collection} to {key}");
        }

        protected async Task<Dictionary<string, object>> LoadData(string collection, string key)
        {
            _Logger.Log($"Loading {collection} from {key}");
            var loadedString = await _Provider.LoadDataAsync(collection, key);
            return _Serializer.Deserialize<Dictionary<string, object>>(loadedString);
        }

        protected async Task DeleteData(string collection, string key)
        {
            _Logger.Log($"Deleting {collection} from {key}");
            await _Provider.DeleteDataAsync(collection, key);
            _Logger.Log($"Deleted {collection}");
        }
    }
}