using System;
using System.Threading;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama.Internal.Authenticate
{
    [CreateAssetMenu(fileName = "DeviceAuthentication", menuName = "ProjectCore/CloudService/Nakama/DeviceAuthentication")]
    public class DeviceAuthentication : Authentication
    {
        public override async Task Authenticate(CancellationToken token,
            Action<bool, ApiResponseException, ISession> callback = null)
        {
            await NakamaServer.AuthenticateWithDeviceID(Callback);
            return;

            async Task Callback(bool success, ApiResponseException exception, ISession session)
            {
                callback?.Invoke(success, exception, session);
                if (!success) return;
                
                await NakamaServer.UpdateSession(session);
                await OnAuthComplete();
            }
        }
        
        public override async Task Link(CancellationToken token,
            Action<bool, ApiResponseException> callback = null)
        {
            await NakamaServer.LinkWithDeviceID(Callback);
            return;
            
            async Task Callback(bool success, ApiResponseException exception)
            {
                callback?.Invoke(success, exception);
                if (!success) return;
                
                await OnLinkComplete();
            }
        }
        
        public override async Task Unlink(CancellationToken token,
            Action<bool, ApiResponseException> callback = null)
        {
            await NakamaServer.UnlinkWithDeviceID(Callback);
            return;
            
            async Task Callback(bool success, ApiResponseException exception)
            {
                callback?.Invoke(success, exception);
                if (!success) return;
                
                await OnUnlinkComplete();
            }
        }

        protected override Task OnAuthComplete()
        {
            return Task.CompletedTask;
        }

        protected override Task OnLinkComplete()
        {
            return Task.CompletedTask;
        }

        protected override Task OnUnlinkComplete()
        {
            return Task.CompletedTask;
        }
    }
}