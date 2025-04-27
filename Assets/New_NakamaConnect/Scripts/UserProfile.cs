using UnityEngine;

namespace ProjectCore.CloudService.Nakama
{
    [CreateAssetMenu(fileName = "UserProfile", menuName = "ProjectCore/SocialFeature/UserProfile")]
    public class UserProfile : ScriptableObject
    {
        public string UserId;
        public string Username;
        public string DisplayName;
        public int AvatarId;

        public bool CanAppearOnline;
    }
}