using ProjectCore.Integrations.Internal;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    public static class UnlinkStrategyFactory
    {
        public static IUnlinkStrategy CreateDeviceUnlinkStrategy() => new DeviceUnlinkStrategy();
        public static IUnlinkStrategy CreateEmailUnlinkStrategy(string email, string password) 
            => new EmailUnlinkStrategy(email, password);
    
        public static IUnlinkStrategy CreateFacebookUnlinkStrategy(string token) 
            => new FacebookUnlinkStrategy(token);
    }
}