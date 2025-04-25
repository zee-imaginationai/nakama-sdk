using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.SocialFeature.Internal
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
}