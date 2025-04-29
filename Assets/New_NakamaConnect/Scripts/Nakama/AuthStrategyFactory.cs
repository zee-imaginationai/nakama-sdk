using ProjectCore.CloudService.Internal;

namespace ProjectCore.CloudService.Nakama.Internal
{
    public static class AuthStrategyFactory
    {
        public static IAuthStrategy CreateDeviceStrategy() => new DeviceAuthStrategy();
    
        public static IAuthStrategy CreateEmailStrategy(string email, string password) 
            => new EmailAuthStrategy(email, password);
    
        public static IAuthStrategy CreateFacebookStrategy(string token) 
            => new FacebookAuthStrategy(token);
    }
}