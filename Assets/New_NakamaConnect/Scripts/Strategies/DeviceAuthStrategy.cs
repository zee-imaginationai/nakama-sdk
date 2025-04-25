using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.SocialFeature.Internal
{
    public class DeviceAuthStrategy : IAuthStrategy
    {
        public async Task<ISession> Authenticate(IClient client, SocialFeatureConfig config)
        {
            string deviceId = config.GetDeviceUdid();
            return await client.AuthenticateDeviceAsync(deviceId, retryConfiguration: config.GetRetryConfiguration());
        }
    }
    
    public class DeviceLinkStrategy : ILinkStrategy
    {
        public async Task Link(ISession session, IClient client, SocialFeatureConfig config)
        {
            string deviceId = config.GetDeviceUdid();
            await client.LinkDeviceAsync(session, deviceId, config.GetRetryConfiguration());
        }
    }
    
    public class DeviceUnlinkStrategy : IUnlinkStrategy
    {
        public async Task Unlink(ISession session, IClient client, SocialFeatureConfig config)
        {
            string deviceId = config.GetDeviceUdid();
            await client.UnlinkDeviceAsync(session, deviceId, config.GetRetryConfiguration());
        }
    }
}