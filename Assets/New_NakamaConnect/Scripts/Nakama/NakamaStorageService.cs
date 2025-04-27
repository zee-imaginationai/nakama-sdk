using Nakama;
using ProjectCore.CloudService.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama.Internal
{
    [CreateAssetMenu(fileName = "NakamaStorageService", menuName = "ProjectCore/CloudService/Nakama/NakamaStorageService")]
    [InlineEditor]
    public class NakamaStorageService : StorageService<IClient, ISession>
    {
        protected override void CreateCloudStorageService()
        {
            NakamaStorageProvider provider = new NakamaStorageProvider(_Client, _Session);
            NakamaSerializationStrategy serializer = new NakamaSerializationStrategy();
            _CloudStorageService = new CloudStorageService(provider, serializer);
        }
    }
}