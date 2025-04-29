using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.CloudService.Internal
{
    public interface IAuthStrategy
    {
        Task<ISession> Authenticate(IClient client, ServerConfig config);
    }
}