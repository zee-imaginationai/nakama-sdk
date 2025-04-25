using System.Threading.Tasks;

namespace ProjectCore.SocialFeature.Internal
{
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

        public async Task Save<T>(string collection, string key, T data)
        {
            var serialized = _serializer.Serialize(data);
            await _provider.SaveDataAsync(collection, key, serialized);
        }

        public async Task<T> Load<T>(string collection, string key)
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