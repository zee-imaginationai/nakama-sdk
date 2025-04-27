using System.Threading.Tasks;

namespace ProjectCore.CloudService.Internal
{
    public interface IAuthStrategy<TSession, in TClient>
    {
        Task<TSession> Authenticate(TClient client, SocialFeatureConfig config);
    }
}