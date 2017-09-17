using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using P7.IdentityServer4.Common.Extensions;

namespace P7.IdentityServer4.Common.Stores
{
    public class DefaultCorsPolicyService : ICorsPolicyService
    {
        private readonly ILogger<InMemoryCorsPolicyService> _logger;
        private readonly IFullClientStore _fullClientStore;

        /// <summary>
        /// The list allowed origins that are allowed.
        /// </summary>
        /// <value>
        /// The allowed origins.
        /// </value>
        private ICollection<string> AllowedOrigins => _allowedOrigins ?? (_allowedOrigins = new List<string>());

        private bool Initialized { get; set; }
        private ICollection<string> _allowedOrigins;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCorsPolicyService"/> class.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="clients">The clients.</param>
        public DefaultCorsPolicyService(ILogger<InMemoryCorsPolicyService> logger,
            IFullClientStore fullClientStore)
        {
            _logger = logger;
            _fullClientStore = fullClientStore;
        }

        private async Task Initialize()
        {
            if (!Initialized)
            {
                AllowedOrigins.Clear();
                Initialized = true;
                byte[] pagingState = null;
                do
                {
                    var page = await _fullClientStore.PageAsync(100, pagingState);
                    pagingState = page.PagingState;
                    var query = from client in page
                        from url in client.AllowedCorsOrigins
                        select url.GetOrigin();
                    foreach (var allowedOrigin in query)
                    {
                        AllowedOrigins.Add(allowedOrigin);
                    }
                } while (pagingState != null);
            }
        }

        /// <summary>
        /// Determines whether origin is allowed.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            await Initialize();

            var result = AllowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);

            if (result)
            {
                _logger.LogDebug("Client list checked and origin: {0} is allowed", origin);
            }
            else
            {
                _logger.LogDebug("Client list checked and origin: {0} is not allowed", origin);
            }
            return result;
        }
    }
}