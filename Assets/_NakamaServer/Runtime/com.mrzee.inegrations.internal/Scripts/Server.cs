using System;
using System.Threading;
using System.Threading.Tasks;
using CustomUtilities.Tools;
using Nakama;

namespace ProjectCore.Integrations.Internal
{
    // Server is explicitly intended to be used as a base class and should not be changed.
    public abstract class Server
    {
        protected ServerConfig _Config;
        protected CustomLogger _Logger;
        
        protected ISocket _Socket;
        protected IApiAccount _Account;

        public IClient Client { get; set; }
        public ISession Session { get; set; }

        protected Server(ServerConfig config, CustomLogger logger)
        {
            _Config = config;
            _Logger = logger;
        }

        public abstract Task Authenticate(IAuthStrategy authStrategy, 
            CancellationToken cancelToken = default, Func<bool, Exception, Task> callback = null);
        public abstract Task Link(ILinkStrategy linkStrategy, 
            CancellationToken cancelToken = default, Func<bool, Exception, Task> callback = null);
        public abstract Task Unlink(IUnlinkStrategy unlinkStrategy, 
            CancellationToken cancelToken = default, Func<bool, Exception, Task> callback = null);
        public abstract void ClearSession();
        public abstract Task<bool> ValidateSession();
        
        protected abstract Task<bool> RefreshSession();
        protected abstract void SaveSession();
        
        protected abstract Task<bool> ValidateSocket();
        
        protected abstract Task UpdateAccount();
        protected abstract Task DeleteAccount();
    }
}
