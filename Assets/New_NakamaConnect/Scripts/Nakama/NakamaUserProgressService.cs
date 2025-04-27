using Nakama;
using ProjectCore.CloudService.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama.Internal
{
    [CreateAssetMenu(fileName = "NakamaUserProgressService", menuName = "ProjectCore/CloudService/Nakama/NakamaUserProgressService")]
    [InlineEditor]
    public class NakamaUserProgressService : UserProgressService<IClient, ISession>
    {
        
    }
}