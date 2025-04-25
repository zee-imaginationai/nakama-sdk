using System;
using System.Threading;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama.Internal.Authenticate
{
    [CreateAssetMenu(fileName = "EmailAuthentication", menuName = "ProjectCore/CloudService/Nakama/EmailAuthentication")]
    public class EmailAuthentication : Authentication
    {
        public override Task Authenticate(CancellationToken token, Action<bool, ApiResponseException, ISession> callback = null)
        {
            throw new System.NotImplementedException();
        }

        public override Task Link(CancellationToken token, Action<bool, ApiResponseException> callback = null)
        {
            throw new System.NotImplementedException();
        }

        public override Task Unlink(CancellationToken token, Action<bool, ApiResponseException> callback = null)
        {
            throw new System.NotImplementedException();
        }

        protected override Task OnAuthComplete()
        {
            throw new NotImplementedException();
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