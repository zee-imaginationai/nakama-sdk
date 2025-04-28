using System;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.Events;
using UnityEngine;
using String = ProjectCore.Variables.String;

namespace ProjectCore.CloudService.Internal
{
    public abstract class Server : ScriptableObject
    {
        [SerializeField] protected SocialFeatureConfig Config;
        
        [SerializeField] protected CustomLogger Logger;
        [SerializeField] protected GameEvent ServerConnectedEvent;
        
        [SerializeField] protected String AuthToken;
        [SerializeField] protected String RefreshToken;

        [SerializeField] protected StorageService StorageService;

        protected IClient _Client;
        protected ISession _Session;
        protected ISocket _Socket;
        
        protected IApiAccount _Account;
        
        public virtual void Initialize()
        {
        }
        
        public abstract Task Authenticate(IAuthStrategy strategy, Func<bool, Exception, ISession, Task> callback = null);
        public abstract Task Link(ILinkStrategy strategy, Func<bool, Exception, Task> callback = null);
        public abstract Task Unlink(IUnlinkStrategy strategy, Func<bool, Exception, Task> callback = null);
        public abstract Task<bool> ValidateSession();
        
        protected abstract Task<bool> ValidateSocket();
        protected abstract Task<bool> RefreshSession();
        protected abstract void SaveSession();
        protected abstract Task UpdateAccount();
        protected abstract Task DeleteAccount();
    }
}
