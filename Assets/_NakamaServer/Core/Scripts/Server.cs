using System;
using System.Threading.Tasks;
using CustomUtilities.Tools;
using Nakama;

namespace ProjectCore.Integrations.Internal
{
    // Server is explicitly intended to be used as a base class and should not be changed.
    internal abstract class Server
    {
        protected ServerConfig _Config;
        protected CustomLogger _Logger;
        
        protected ISocket _Socket;
        protected IApiAccount _Account;

        internal IClient Client { get; set; }
        internal ISession Session { get; set; }

        protected Server(ServerConfig config, CustomLogger logger)
        {
            _Config = config;
            _Logger = logger;
        }

        internal abstract Task Authenticate(IAuthStrategy authStrategy, Func<bool, Exception, Task> callback = null);
        internal abstract Task Link(ILinkStrategy linkStrategy, Func<bool, Exception, Task> callback = null);
        internal abstract Task Unlink(IUnlinkStrategy unlinkStrategy, Func<bool, Exception, Task> callback = null);
        internal abstract void ClearSession();
        internal abstract Task<bool> ValidateSession();
        
        protected abstract Task<bool> RefreshSession();
        protected abstract void SaveSession();
        
        protected abstract Task<bool> ValidateSocket();
        
        protected abstract Task UpdateAccount();
        protected abstract Task DeleteAccount();
    }
}
