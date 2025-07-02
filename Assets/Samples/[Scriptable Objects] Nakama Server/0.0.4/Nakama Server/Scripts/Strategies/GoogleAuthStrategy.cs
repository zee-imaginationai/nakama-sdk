using System.Threading;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.Integrations.Internal;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    public class GoogleAuthStrategy : IAuthStrategy
    {
        private readonly string _token;

        public GoogleAuthStrategy(string token)
        {
            _token = token;
        }
        
        public async Task<ISession> Authenticate(IClient client, CancellationToken cancelToken,
            ServerConfig config)
        {
            return await client.AuthenticateGoogleAsync(_token, create: true,
                retryConfiguration: config.GetRetryConfiguration(), canceller: cancelToken);
        }
    }
    
    public class GoogleLinkStrategy : ILinkStrategy
    {
        private readonly string _token;

        public GoogleLinkStrategy(string token)
        {
            _token = token;
        }
        
        public async Task Link(ISession session, IClient client, CancellationToken cancelToken,
            ServerConfig config)
        {
            await client.LinkGoogleAsync(session, _token,
                retryConfiguration: config.GetRetryConfiguration(), canceller: cancelToken);
        }
    }
    
    public class GoogleUnlinkStrategy : IUnlinkStrategy
    {
        private readonly string _token;

        public GoogleUnlinkStrategy(string token)
        {
            _token = token;
        }
        
        public async Task Unlink(ISession session, IClient client, CancellationToken cancelToken,
            ServerConfig config)
        {
            await client.UnlinkGoogleAsync(session, _token,
                retryConfiguration: config.GetRetryConfiguration(), canceller: cancelToken);
        }
    }
}