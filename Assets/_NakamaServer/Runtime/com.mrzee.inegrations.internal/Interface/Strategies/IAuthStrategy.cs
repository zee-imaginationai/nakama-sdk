using System.Threading;
using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.Integrations.Internal
{
    public interface IAuthStrategy
    {
        Task<ISession> Authenticate(IClient client, CancellationToken cancelToken, ServerConfig config);
    }
}