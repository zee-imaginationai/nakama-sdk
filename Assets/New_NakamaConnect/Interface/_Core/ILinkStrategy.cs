using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.Integrations.Internal
{
    public interface ILinkStrategy
    {
        Task Link(ISession session, IClient client, ServerConfig config);
    }
}