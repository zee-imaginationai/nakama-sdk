using System.Linq;
using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.SocialFeature.Internal
{
    // (Abstract interface for any backend)
    public interface IStorageProvider
    {
        Task SaveDataAsync(string collection, string key, string value);
        Task<string> LoadDataAsync(string collection, string key);
        Task DeleteDataAsync(string collection, string key);
    }
    
    public class NakamaStorageProvider : IStorageProvider
    {
        private readonly IClient _client;
        private readonly ISession _session;

        public NakamaStorageProvider(IClient client, ISession session)
        {
            _client = client;
            _session = session;
        }

        public async Task SaveDataAsync(string collection, string key, string value)
        {
            var writeObject = new WriteStorageObject
            {
                Collection = collection,
                Key = key,
                Value = value
            };
            await _client.WriteStorageObjectsAsync(_session, new IApiWriteStorageObject[] { writeObject });
        }

        public async Task<string> LoadDataAsync(string collection, string key)
        {
            var storageObject = new StorageObjectId()
            {
                Collection = collection,
                Key = key,
                UserId = _session.UserId
            };
            var result = await _client.ReadStorageObjectsAsync(_session, new IApiReadStorageObjectId[]{storageObject});
            return result.Objects.FirstOrDefault()?.Value;
        }

        public async Task DeleteDataAsync(string collection, string key)
        {
            var storageObject = new StorageObjectId()
            {
                Collection = collection,
                Key = key,
                UserId = _session.UserId
            };
            await _client.DeleteStorageObjectsAsync(_session, new StorageObjectId[]{storageObject});
        }
    }
}