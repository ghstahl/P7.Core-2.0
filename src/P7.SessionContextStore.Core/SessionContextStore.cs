using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace P7.SessionContextStore.Core
{
    public class SessionContextStore : ISessionContextStore
    {
        static ILogger logger = Log.ForContext<SessionContextStore>();
        private const string SessionContextKey = "67bd2a4e-df62-4ea3-a6ef-16ed31566039";
        Dictionary<string, ILocalSessionContext> SessionContexts = new Dictionary<string, ILocalSessionContext>();
        private IRemoteSessionContextAccessor _remoteSessionContextAccessor;
        private IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
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
            //Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the semaphore is released 
            await _semaphoreSlim.WaitAsync();
            try
            {
                var contextKey = Session.GetString(SessionContextKey);
                ILocalSessionContext localSessionContext;
                if (string.IsNullOrEmpty(contextKey))
                {
                    logger.Information("GetSessionContextAsync contextKey is null");
                    // This is the first time, so get the latest contextKey from remote
                    contextKey = await _remoteSessionContextAccessor.GetCurrentContextKeyAsync();
                    Session.SetString(SessionContextKey, contextKey);
                    localSessionContext = _serviceProvider
                        .GetServices<ILocalSessionContext>()
                        .First();

                    localSessionContext.SetContextKey(contextKey);

                    // Important, it may have alread been added a moment before

                    var added = SessionContexts.TryAdd(contextKey, localSessionContext);
                    if (!added)
                    {
                        // only get here if a sessionless request got here before we did
                        if (SessionContexts.TryGetValue(contextKey, out localSessionContext))
                        {
                            logger.Information("GetSessionContextAsync TryGetValue is in Dictionary");
                            // all is good and things are working post creation
                            return localSessionContext;
                        }
                        throw new Exception("remote session context is bad.  You probably need to trigger a logout event");
                    }
                    return localSessionContext;
                }

                if (SessionContexts.TryGetValue(contextKey, out localSessionContext))
                {
                    logger.Information("GetSessionContextAsync TryGetValue is in Dictionary");
                    // all is good and things are working post creation
                    return localSessionContext;
                }


                // we get here because we probably are using a distributed session cache
                var exists = await _remoteSessionContextAccessor.GetContextKeyExistsAsync(contextKey);
                if (exists)
                {
                    logger.Information("GetSessionContextAsync GetContextKeyExistsAsync");
                    // if we got here, it was because some other app created the ISessionContext, we just need to create it here.
                    localSessionContext = _serviceProvider.GetServices<ILocalSessionContext>().First();
                    localSessionContext.SetContextKey(contextKey);
                    SessionContexts.Add(contextKey, localSessionContext);
                    return localSessionContext;

                }
                logger.Information("GetSessionContextAsync All is lost");
                // if we get here, things have gone off the rails on the remote side.
                // kill this session and start over.
                throw new Exception("remote session context is bad.  You probably need to trigger a logout event");
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }
        }
    }
}