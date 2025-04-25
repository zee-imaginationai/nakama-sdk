using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.SocialFeature.Internal
{
    public class NakamaCloudRepository : IUserProgressRepository
    {
        private readonly IClient _client;
        private readonly ISession _session;

        public NakamaCloudRepository(IClient client, ISession session)
        {
            _client = client;
            _session = session;
        }

        public async Task SaveAsync(IApiWriteStorageObject[] objects)
        {
            await _client.WriteStorageObjectsAsync(_session, objects);
        }

        public async Task<IApiStorageObjects> LoadAsync(IApiReadStorageObjectId[] objectIds)
        {
            return await _client.ReadStorageObjectsAsync(_session, objectIds);
        }

        public async Task DeleteAsync(StorageObjectId[] objectIds)
        {
            await _client.DeleteStorageObjectsAsync(_session, objectIds);
        }
    }
}