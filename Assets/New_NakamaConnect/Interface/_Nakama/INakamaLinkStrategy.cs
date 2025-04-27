using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.CloudService.Nakama.Internal
{
    public interface INakamaLinkStrategy : ILinkStrategy<ISession, IClient>
    {
        new Task Link(ISession session, IClient client, SocialFeatureConfig config);
    }
}