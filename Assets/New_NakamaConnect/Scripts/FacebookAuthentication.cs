using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nakama;
using Nakama.TinyJson;
using ProjectCore.Variables;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama.Internal.Authenticate
{
    [CreateAssetMenu(fileName = "FacebookAuthentication", menuName = "ProjectCore/CloudService/Nakama/FacebookAuthentication")]
    public class FacebookAuthentication : Authentication
    {
        [SerializeField] private DBString FbAccessToken;
        public override async Task Authenticate(CancellationToken token, Action<bool, ApiResponseException, ISession> callback = null)
        {
            await NakamaServer.AuthenticateWithFacebook(FbAccessToken.GetValue(), Callback);
            return;
            
            async Task Callback(bool success, ApiResponseException exception, ISession session)
            {
                callback?.Invoke(success, exception, session);
                if (!success) return;
                
                await NakamaServer.UpdateSession(session);
                await OnAuthComplete();
            }
        }

        public override async Task Link(CancellationToken token, Action<bool, ApiResponseException> callback = null)
        {
            await NakamaServer.LinkWithFacebook(FbAccessToken.GetValue(), Callback);
            return;
            
            async Task Callback(bool success, ApiResponseException exception)
            {
                callback?.Invoke(success, exception);
                if (!success) return;
                
                await OnLinkComplete();
            }
        }

        public override async Task Unlink(CancellationToken token, Action<bool, ApiResponseException> callback = null)
        {
            await NakamaServer.UnlinkWithFacebook(FbAccessToken.GetValue(), Callback);
            return;
            
            async Task Callback(bool success, ApiResponseException exception)
            {
                callback?.Invoke(success, exception);
                if (!success) return;
                
                await OnUnlinkComplete();
            }
        }

        protected override async Task OnAuthComplete()
        {
            var data = await UserProfileService.GetUserData();
            
            Debug.LogError($"[CloudSyncSystem] UserHasData: {UserHasData(data)}");
            
            if (UserHasData(data))
            {
                var userProgress = data.Objects.First()?.Value.FromJson<Dictionary<string, object>>();
                
                if(userProgress == null) return;
                
                DBManager.LoadJsonData(userProgress);
            }
        }

        protected override Task OnLinkComplete()
        {
            throw new NotImplementedException();
        }

        protected override Task OnUnlinkComplete()
        {
            throw new NotImplementedException();
        }
    }
}