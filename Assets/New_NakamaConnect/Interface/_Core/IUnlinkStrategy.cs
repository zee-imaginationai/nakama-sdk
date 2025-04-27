using System.Threading.Tasks;

namespace ProjectCore.CloudService.Internal
{
    public interface IUnlinkStrategy<TSession, in TClient>
    {
        Task Unlink(TSession session, TClient client, SocialFeatureConfig config);
    }
}