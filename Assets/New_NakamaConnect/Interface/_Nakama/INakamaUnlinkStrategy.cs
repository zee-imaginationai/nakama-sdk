using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.CloudService.Nakama.Internal
{
    public interface INakamaUnlinkStrategy :  IUnlinkStrategy<ISession, IClient>
    {
        new Task Unlink(ISession session, IClient client, SocialFeatureConfig config);
    }
}