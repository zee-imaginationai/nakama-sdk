using Nakama;
using ProjectCore.Integrations.Internal;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    public static class LinkStrategyFactory
    {
        public static ILinkStrategy CreateDeviceLinkStrategy() => new DeviceLinkStrategy();
        public static ILinkStrategy CreateEmailLinkStrategy(string email, string password) 
            => new EmailLinkStrategy(email, password);
    
        public static ILinkStrategy CreateFacebookLinkStrategy(string token, ISession session) 
            => new FacebookLinkStrategy(token, session);
    }
}