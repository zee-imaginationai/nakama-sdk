using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.CloudService.Internal
{
    public interface ILinkStrategy
    {
        Task Link(ISession session, IClient client, ServerConfig config);
    }
}