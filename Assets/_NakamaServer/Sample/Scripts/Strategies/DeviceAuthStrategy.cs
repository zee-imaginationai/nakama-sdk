using System.Threading.Tasks;
using Nakama;
using ProjectCore.Integrations.Internal;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    internal class DeviceAuthStrategy : IAuthStrategy
    {
        public async Task<ISession> Authenticate(IClient client, ServerConfig config)
        {
            string deviceId = config.GetDeviceUdid();
            return await client.AuthenticateDeviceAsync(deviceId, retryConfiguration: config.GetRetryConfiguration());
        }
    }

    internal class DeviceLinkStrategy : ILinkStrategy
    {
        public async Task Link(ISession session, IClient client, ServerConfig config)
        {
            string deviceId = config.GetDeviceUdid();
            await client.LinkDeviceAsync(session, deviceId, config.GetRetryConfiguration());
        }
    }

    internal class DeviceUnlinkStrategy : IUnlinkStrategy
    {
        public async Task Unlink(ISession session, IClient client, ServerConfig config)
        {
            string deviceId = config.GetDeviceUdid();
            await client.UnlinkDeviceAsync(session, deviceId, config.GetRetryConfiguration());
        }
    }
}