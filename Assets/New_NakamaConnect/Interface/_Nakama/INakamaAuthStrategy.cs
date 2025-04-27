using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.CloudService.Nakama.Internal
{
    public interface INakamaAuthStrategy : IAuthStrategy<ISession, IClient>
    {
        new Task<ISession> Authenticate(IClient client, SocialFeatureConfig config);
    }
}

