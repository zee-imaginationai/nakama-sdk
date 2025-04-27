using System.Threading.Tasks;

namespace ProjectCore.CloudService.Nakama.Internal
{
    public interface IUnlinkStrategy<TSession, in TClient>
    {
        Task Unlink(TSession session, TClient client, SocialFeatureConfig config);
    }
}