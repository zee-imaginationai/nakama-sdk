using ProjectCore.Integrations.Internal;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    internal static class UnlinkStrategyFactory
    {
        internal static IUnlinkStrategy CreateDeviceUnlinkStrategy() => new DeviceUnlinkStrategy();

        internal static IUnlinkStrategy CreateEmailUnlinkStrategy(string email, string password) 
            => new EmailUnlinkStrategy(email, password);

        internal static IUnlinkStrategy CreateFacebookUnlinkStrategy(string token) 
            => new FacebookUnlinkStrategy(token);
    }
}