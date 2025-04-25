using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.SocialFeature.Internal
{
    public interface IAuthStrategy
    {
        Task<ISession> Authenticate(IClient client, SocialFeatureConfig config);
    }

    public interface ILinkStrategy
    {
        Task Link(ISession session, IClient client, SocialFeatureConfig config);
    }
    
    public interface IUnlinkStrategy
    {
        Task Unlink(ISession session, IClient client, SocialFeatureConfig config);
    }
}