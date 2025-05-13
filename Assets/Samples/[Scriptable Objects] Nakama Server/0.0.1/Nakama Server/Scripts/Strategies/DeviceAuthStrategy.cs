using System.Threading;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.Integrations.Internal;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    internal class DeviceAuthStrategy : IAuthStrategy
    {
        public async Task<ISession> Authenticate(IClient client, CancellationToken cancelToken,
            ServerConfig config)
        {
            string deviceId = config.GetDeviceUdid();
            return await client.AuthenticateDeviceAsync(deviceId, canceller: cancelToken, 
                retryConfiguration: config.GetRetryConfiguration());
        }
    }

    internal class DeviceLinkStrategy : ILinkStrategy
    {
        public async Task Link(ISession session, IClient client, 
            CancellationToken cancelToken, ServerConfig config)
        {
            string deviceId = config.GetDeviceUdid();
            await client.LinkDeviceAsync(session, deviceId, config.GetRetryConfiguration(), cancelToken);
        }
    }

    internal class DeviceUnlinkStrategy : IUnlinkStrategy
    {
        public async Task Unlink(ISession session, IClient client, 
            CancellationToken cancelToken, ServerConfig config)
        {
            string deviceId = config.GetDeviceUdid();
            await client.UnlinkDeviceAsync(session, deviceId, config.GetRetryConfiguration(), cancelToken);
        }
    }
}