using System.Threading.Tasks;

namespace ProjectCore.CloudService.Internal
{
    public interface IStorageProvider
    {
        Task SaveDataAsync(string collection, string key, string value);
        Task<string> LoadDataAsync(string collection, string key);
        Task DeleteDataAsync(string collection, string key);
    }
    
    public class CloudStorageService
    {
        private readonly IStorageProvider _provider;
        private readonly ISerializationStrategy _serializer;

        public CloudStorageService(
            IStorageProvider provider, 
            ISerializationStrategy serializer = null
        )
        {
            _provider = provider;
            _serializer = serializer ?? new JsonSerializationStrategy();
        }

        public async Task Save(string collection, string key, string data)
        {
            var serialized = _serializer.Serialize(data);
            await _provider.SaveDataAsync(collection, key, serialized);
        }

        public async Task<T> Load<T>(string collection, string key) where T : class
        {
            var serialized = await _provider.LoadDataAsync(collection, key);
            return _serializer.Deserialize<T>(serialized);
        }

        public async Task Delete(string collection, string key)
        {
            await _provider.DeleteDataAsync(collection, key);
        }
    }
}