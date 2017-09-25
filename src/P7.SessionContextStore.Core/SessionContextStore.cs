using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace P7.SessionContextStore.Core
{
    public class SessionContextStore : ISessionContextStore
    {
        private const string SessionContextKey = "67bd2a4e-df62-4ea3-a6ef-16ed31566039";
        Dictionary<string,ISessionContext> SessionContexts = new Dictionary<string, ISessionContext>();
        private IRemoteSessionContextAccessor _remoteSessionContextAccessor;
        private IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        public SessionContextStore(
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor, 
            IRemoteSessionContextAccessor remoteSessionContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
            _remoteSessionContextAccessor = remoteSessionContextAccessor;
        }

        public async Task<ISessionContext> GetSessionContextAsync()
        {
            var contextKey = SessionExtensions.GetString(Session, SessionContextKey);
            ISessionContext sessionContext;
            if (string.IsNullOrEmpty(contextKey))
            {
                // This is the first time, so get the latest contextKey from remote
                contextKey = await _remoteSessionContextAccessor.GetCurrentContextKeyAsync();
                SessionExtensions.SetString(Session, SessionContextKey,contextKey);
                sessionContext = ServiceProviderServiceExtensions.GetServices<ISessionContext>(_serviceProvider).First();
                sessionContext.SetContextKey(contextKey);
                SessionContexts.Add(contextKey, sessionContext);
                return sessionContext;
            }

            if (SessionContexts.TryGetValue(contextKey, out sessionContext))
            {
                // all is good and things are working post creation
                return sessionContext;
            }


            // we get here because we probably are using a distributed session cache
            var exists = await _remoteSessionContextAccessor.GetContextKeyExistsAsync(contextKey);
            if (exists)
            {
                // if we got here, it was because some other app created the ISessionContext, we just need to create it here.
                sessionContext = ServiceProviderServiceExtensions.GetServices<ISessionContext>(_serviceProvider).First();
                sessionContext.SetContextKey(contextKey);
                SessionContexts.Add(contextKey, sessionContext);
                return sessionContext; 

            }

            // if we get here, things have gone off the rails on the remote side.
            // kill this session and start over.
            throw new Exception("remote session context is bad.  You probably need to trigger a logout event");
        }
    }
}