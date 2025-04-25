using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nakama;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.CloudService.Nakama.Internal.Authenticate
{
    [InlineEditor]
    public abstract class Authentication : ScriptableObject
    {
        [SerializeField] protected NakamaServer NakamaServer;
        [SerializeField] protected UserProfileService UserProfileService;
        
        [Button]
        public abstract Task Authenticate(CancellationToken token, Action<bool, ApiResponseException, ISession> callback = null);
        
        [Button]
        public abstract Task Link(CancellationToken token, Action<bool, ApiResponseException> callback = null);
        
        [Button]
        public abstract Task Unlink(CancellationToken token, Action<bool, ApiResponseException> callback = null);
        
        protected abstract Task OnAuthComplete();
        protected abstract Task OnLinkComplete();
        protected abstract Task OnUnlinkComplete();
        
        protected bool UserHasData(IApiStorageObjects obj)
        {
            if (obj == null) return false;
            var data = obj.Objects.ToList();
            return data.Count != 0;
        }
    }
}