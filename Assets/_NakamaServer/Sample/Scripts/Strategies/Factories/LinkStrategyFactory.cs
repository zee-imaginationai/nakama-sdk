using Nakama;
using ProjectCore.Integrations.Internal;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    internal static class LinkStrategyFactory
    {
        internal static ILinkStrategy CreateDeviceLinkStrategy() => new DeviceLinkStrategy();

        internal static ILinkStrategy CreateEmailLinkStrategy(string email, string password) 
            => new EmailLinkStrategy(email, password);

        internal static ILinkStrategy CreateFacebookLinkStrategy(string token, ISession session) 
            => new FacebookLinkStrategy(token, session);
    }
}