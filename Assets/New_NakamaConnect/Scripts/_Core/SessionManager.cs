using System;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;
using String = ProjectCore.Variables.String;

namespace ProjectCore.CloudService.Internal
{
    public abstract class SessionManager<TClient, TSession, TSocket, TAccount> : ScriptableObject
    {
        [NonSerialized] public TAccount Account;
        [NonSerialized] public TSession Session;

        [SerializeField] protected CustomLogger Logger;
        [SerializeField] protected String AuthToken, RefreshToken;

        [SerializeField] protected SocialFeatureConfig SocialFeatureConfig;

        [SerializeField] protected StorageService<IClient, ISession> StorageService;

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