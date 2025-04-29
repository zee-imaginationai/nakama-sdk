using System.Threading.Tasks;
using Nakama;
using ProjectCore.CloudService.Internal;

namespace ProjectCore.CloudService.Nakama.Internal
{
    public class DeviceAuthStrategy : INakamaAuthStrategy
    {
        public async Task<ISession> Authenticate(IClient client, SocialFeatureConfig config)
        {
            string deviceId = config.GetDeviceUdid();
            return await client.AuthenticateDeviceAsync(deviceId, retryConfiguration: config.GetRetryConfiguration());
        }
    }
    
    public class DeviceLinkStrategy : INakamaLinkStrategy
    {
        public async Task Link(ISession session, IClient client, SocialFeatureConfig config)
        {
            string deviceId = config.GetDeviceUdid();
            await client.LinkDeviceAsync(session, deviceId, config.GetRetryConfiguration());
        }
    }
    
    public class DeviceUnlinkStrategy : INakamaUnlinkStrategy
    {
        public async Task Unlink(ISession session, IClient client, SocialFeatureConfig config)
        {
            string deviceId = config.GetDeviceUdid();
            await client.UnlinkDeviceAsync(session, deviceId, config.GetRetryConfiguration());
        }
    }
}