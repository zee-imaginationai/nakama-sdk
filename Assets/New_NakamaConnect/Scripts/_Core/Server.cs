using System;
using System.Threading.Tasks;
using ProjectCore.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using String = ProjectCore.Variables.String;

namespace ProjectCore.CloudService.Nakama.Internal
{
    [InlineEditor]
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
    
    [InlineEditor]
    public abstract class SessionManager<TClient, TSession, TSocket, TAccount> : ScriptableObject
    {
        [NonSerialized] public TAccount Account;
        [NonSerialized] public TSession Session;

        [SerializeField] protected CustomLogger Logger;
        [SerializeField] protected String AuthToken, RefreshToken;

        [SerializeField] protected SocialFeatureConfig SocialFeatureConfig;

        [SerializeField] protected StorageService StorageService;

        protected TSocket _Socket;
        protected TClient _Client;
        
        protected abstract Task<bool> ValidateSocket();
        protected abstract Task<bool> RefreshSession();
        protected abstract void SaveSession();
        protected abstract Task UpdateAccount();
        protected abstract Task DeleteAccount();

        public virtual void InitializeManager(TClient client)
        {
            _Client = client;
        }

        public virtual Task InitializeSession(TClient client, TSession session)
        {
            _Client = client;
            Session = session;
            return Task.CompletedTask;
        }
        
        public abstract Task Disconnect();
        public abstract Task<bool> ValidateSession();

    }
}
