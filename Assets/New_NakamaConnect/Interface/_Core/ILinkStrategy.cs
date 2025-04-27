using System.Threading.Tasks;

namespace ProjectCore.CloudService.Internal
{
    public interface ILinkStrategy<in TSession, in TClient>
    {
        Task Link(TSession session, TClient client, SocialFeatureConfig config);
    }
}