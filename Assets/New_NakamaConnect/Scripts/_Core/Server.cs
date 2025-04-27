using System;
using System.Threading.Tasks;
using ProjectCore.Events;
using UnityEngine;

namespace ProjectCore.CloudService.Internal
{
    public abstract class Server<TClient, TSession, TSocket, TAccount> : ScriptableObject
    {
        [SerializeField] protected SessionManager<TClient, TSession, TSocket, TAccount> SessionManager;
        [SerializeField] protected SocialFeatureConfig SocialFeatureConfig;
        [SerializeField] protected CustomLogger Logger;
        [SerializeField] protected GameEvent ServerConnectedEvent;
        
        public abstract Task Authenticate(IAuthStrategy<TSession, TClient> strategy, Func<bool, Exception, TSession, Task> callback = null);
        public abstract Task Link(ILinkStrategy<TSession, TClient> strategy, Func<bool, Exception, Task> callback = null);
        public abstract Task Unlink(IUnlinkStrategy<TSession, TClient> strategy, Func<bool, Exception, Task> callback = null);
    }
}
