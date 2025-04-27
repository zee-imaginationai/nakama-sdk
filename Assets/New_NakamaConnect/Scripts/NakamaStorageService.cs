using ProjectCore.CloudService.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama.Internal
{
    [CreateAssetMenu(fileName = "NakamaStorageService", menuName = "ProjectCore/SocialFeature/Cloud/NakamaStorageService")]
    [InlineEditor]
    public class NakamaStorageService : StorageService
    {
        protected override void CreateCloudStorageService()
        {
            NakamaStorageProvider provider = new NakamaStorageProvider(_Client, _Session);
            NakamaSerializationStrategy serializer = new NakamaSerializationStrategy();
            _CloudStorageService = new CloudStorageService(provider, serializer);
        }
    }
}