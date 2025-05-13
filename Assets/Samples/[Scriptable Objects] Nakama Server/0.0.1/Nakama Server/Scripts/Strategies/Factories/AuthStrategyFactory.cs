using ProjectCore.Integrations.Internal;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    internal static class AuthStrategyFactory
    {
        internal static IAuthStrategy CreateDeviceStrategy() => new DeviceAuthStrategy();

        internal static IAuthStrategy CreateEmailStrategy(string email, string password) 
            => new EmailAuthStrategy(email, password);

        internal static IAuthStrategy CreateFacebookStrategy(string token) 
            => new FacebookAuthStrategy(token);
    }
}