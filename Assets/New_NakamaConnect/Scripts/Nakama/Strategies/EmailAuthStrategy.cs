using System.Threading.Tasks;
using ProjectCore.CloudService.Internal;
using Nakama;

namespace ProjectCore.CloudService.Nakama.Internal
{
    public class EmailAuthStrategy : IAuthStrategy
    {
        private readonly string _email;
        private readonly string _password;

        public EmailAuthStrategy(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public async Task<ISession> Authenticate(IClient client, SocialFeatureConfig config)
        {
            return await client.AuthenticateEmailAsync(_email, _password, create: false, 
                retryConfiguration: config.GetRetryConfiguration());
        }
    }

    public class EmailLinkStrategy : ILinkStrategy
    {
        private readonly string _email;
        private readonly string _password;

        public EmailLinkStrategy(string email, string password)
        {
            _email = email;
            _password = password;
        }
        
        public async Task Link(ISession session, IClient client, SocialFeatureConfig config)
        {
            await client.LinkEmailAsync(session, _email, _password, config.GetRetryConfiguration());
        }
    }
    
    public class EmailUnlinkStrategy : IUnlinkStrategy
    {
        private readonly string _email;
        private readonly string _password;

        public EmailUnlinkStrategy(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public async Task Unlink(ISession session, IClient client, SocialFeatureConfig config)
        {
            await client.UnlinkEmailAsync(session, _email, _password, config.GetRetryConfiguration());
        }   
    }
}