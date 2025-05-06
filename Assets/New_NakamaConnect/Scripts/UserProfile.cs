using UnityEngine;

namespace ProjectCore.Integrations.NakamaServer
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