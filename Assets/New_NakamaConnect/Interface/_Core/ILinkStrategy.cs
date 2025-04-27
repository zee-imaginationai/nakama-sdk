using System.Threading.Tasks;

namespace ProjectCore.CloudService.Nakama.Internal
{
    public interface ILinkStrategy<in TSession, in TClient>
    {
        Task Link(TSession session, TClient client, SocialFeatureConfig config);
    }
}