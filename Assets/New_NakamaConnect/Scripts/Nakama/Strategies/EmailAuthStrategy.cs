using System.Threading.Tasks;
using Nakama;
using ProjectCore.Integrations.Internal;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    internal class EmailAuthStrategy : IAuthStrategy
    {
        private readonly string _email;
        private readonly string _password;

        internal EmailAuthStrategy(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public async Task<ISession> Authenticate(IClient client, ServerConfig config)
        {
            return await client.AuthenticateEmailAsync(_email, _password, create: false, 
                retryConfiguration: config.GetRetryConfiguration());
        }
    }

    internal class EmailLinkStrategy : ILinkStrategy
    {
        private readonly string _email;
        private readonly string _password;

        internal EmailLinkStrategy(string email, string password)
        {
            _email = email;
            _password = password;
        }
        
        public async Task Link(ISession session, IClient client, ServerConfig config)
        {
            await client.LinkEmailAsync(session, _email, _password, config.GetRetryConfiguration());
        }
    }

    internal class EmailUnlinkStrategy : IUnlinkStrategy
    {
        private readonly string _email;
        private readonly string _password;

        internal EmailUnlinkStrategy(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public async Task Unlink(ISession session, IClient client, ServerConfig config)
        {
            await client.UnlinkEmailAsync(session, _email, _password, config.GetRetryConfiguration());
        }   
    }
}