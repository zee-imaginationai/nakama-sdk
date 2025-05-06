using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.Integrations.Internal
{
    public interface IUnlinkStrategy
    {
        Task Unlink(ISession session, IClient client, ServerConfig config);
    }
}