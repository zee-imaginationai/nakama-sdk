using System.Threading;
using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.Integrations.Internal
{
    public interface ILinkStrategy
    {
        Task Link(ISession session, IClient client, CancellationToken cancelToken, ServerConfig config);
    }
}