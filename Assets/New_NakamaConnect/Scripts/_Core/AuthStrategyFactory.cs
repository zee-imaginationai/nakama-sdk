namespace ProjectCore.CloudService.Nakama.Internal
{
    public static class AuthStrategyFactory
    {
        public static INakamaAuthStrategy CreateDeviceStrategy() => new DeviceAuthStrategy();
    
        public static INakamaAuthStrategy CreateEmailStrategy(string email, string password) 
            => new EmailNakamaAuthStrategy(email, password);
    
        public static INakamaAuthStrategy CreateFacebookStrategy(string token) 
            => new FacebookAuthStrategy(token);
    }
}