using System.Threading.Tasks;
using Nakama;
using ProjectCore.CloudService.Internal;

namespace ProjectCore.CloudService.Nakama.Internal
{
    public class EmailNakamaAuthStrategy : INakamaAuthStrategy
    {
        private readonly string _email;
        private readonly string _password;

        public EmailNakamaAuthStrategy(string email, string password)
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