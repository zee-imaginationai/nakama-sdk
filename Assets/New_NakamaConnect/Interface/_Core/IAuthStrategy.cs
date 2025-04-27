using System.Threading.Tasks;

namespace ProjectCore.CloudService.Nakama.Internal
{
    public interface IAuthStrategy<TSession, in TClient>
    {
        Task<TSession> Authenticate(TClient client, SocialFeatureConfig config);
    }
}