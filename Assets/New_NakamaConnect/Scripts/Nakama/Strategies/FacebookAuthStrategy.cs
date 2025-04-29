using System.Threading.Tasks;
using Nakama;
using ProjectCore.CloudService.Internal;

namespace ProjectCore.CloudService.Nakama.Internal
{
    public class FacebookAuthStrategy : IAuthStrategy //IAuthStrategy<ISession, IClient>
    {
        private readonly string _token;

        public FacebookAuthStrategy(string token)
        {
            _token = token;
        }

        public async Task<ISession> Authenticate(IClient client, ServerConfig config)
        {
            return await client.AuthenticateFacebookAsync(_token, create: true, 
                retryConfiguration: config.GetRetryConfiguration());
        }
    }

    public class FacebookLinkStrategy : ILinkStrategy
    {
        private readonly string _token;
        
        public FacebookLinkStrategy(string token, ISession session)
        {
            _token = token;
        }
        
        public async Task Link(ISession session, IClient client, ServerConfig config)
        {
            await client.LinkFacebookAsync(session, _token, retryConfiguration: config.GetRetryConfiguration());
        }
    }
    
    public class FacebookUnlinkStrategy : IUnlinkStrategy
    {
        private readonly string _token;
        
        public FacebookUnlinkStrategy(string token)
        {
            _token = token;
        }
        
        public async Task Unlink(ISession session, IClient client, ServerConfig config)
        {
            await client.UnlinkFacebookAsync(session, _token, retryConfiguration: config.GetRetryConfiguration());
        }
    }
}