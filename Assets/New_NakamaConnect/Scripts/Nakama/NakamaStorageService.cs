using System.Collections.Generic;
using System.Threading.Tasks;
using CustomUtilities.Tools;
using Nakama;
using ProjectCore.CloudService.Internal;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama.Internal
{
    [CreateAssetMenu(fileName = "NakamaStorageService", menuName = "ProjectCore/CloudService/Nakama/NakamaStorageService")]
    [InlineEditor]
    public class NakamaStorageService : ScriptableObject
    {
        private IStorageProvider _provider;
        private ISerializationStrategy _serializer;
        private CustomLogger _logger;
        
        private const string COLLECTION_NAME = "Save";
        private const string KEY_NAME = "UserProgress";
        
        public void Initialize(IClient client, ISession session, CustomLogger logger)
        {
            _logger = logger;
            _provider = new NakamaStorageProvider(client, session);
            _serializer = new NakamaSerializationStrategy();
        }

        public async Task SaveUserProgress()
        {
            try
            {
                var data = DBManager.GetJsonData();
                await SaveData(COLLECTION_NAME, KEY_NAME, data);
            }
            catch
            {
                _logger.LogCritical("[Nakama] Failed to save user data");
            }
        }

        public async Task<Dictionary<string, object>> LoadUserProgress()
        {
            try
            {
                return await LoadData(COLLECTION_NAME, KEY_NAME);
            }
            catch
            {
                _logger.LogCritical("[Nakama] Failed to load user data");
                return new Dictionary<string, object>();
            }
        }

        public async Task DeleteUserProgress()
        {
            try
            {
                await DeleteData(COLLECTION_NAME, KEY_NAME);
            }
            catch
            {
                _logger.LogCritical("[Nakama] Failed to load user data");
            }
        }
        
        private async Task SaveData(string collection, string key, string data)
        {
            _logger.Log($"Saving {collection} to {key}]");
            await _provider.SaveDataAsync(collection, key, data);
            _logger.Log($"Saved {collection} to {key}");
        }

        private async Task<Dictionary<string, object>> LoadData(string collection, string key)
        {
            _logger.Log($"Loading {collection} from {key}");
            var loadedString = await _provider.LoadDataAsync(collection, key);
            return _serializer.Deserialize<Dictionary<string, object>>(loadedString);
        }

        private async Task DeleteData(string collection, string key)
        {
            _logger.Log($"Deleting {collection} from {key}");
            await _provider.DeleteDataAsync(collection, key);
            _logger.Log($"Deleted {collection}");
        }
    }
}