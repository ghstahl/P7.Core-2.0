using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using P7.Core.Scheduler.Scheduling;
using P7.Core.Utils;
using P7.External.SPA.Core;
using P7.HealthCheck.Core;
using P7.RazorProvider.Store.Core;
using P7.RazorProvider.Store.Core.Models;
using Serilog;

namespace P7.External.SPA.Scheduler
{
    public class RemoteStaticExternalSpaStoreTask : IScheduledTask
    {
        private const string Url = "https://rawgit.com/ghstahl/P7.Core-2.0/master/RemoteData/external.spa.config.json";
        private IRemoteExternalSPAStore RemoteExternalSPAStore { get; set; }
        private IHealthCheckStore HealthCheckStore { get; set; }
        private IConfiguration _config { get; set; }
        static Serilog.ILogger logger = Log.ForContext<RemoteStaticExternalSpaStoreTask>();
        public RemoteStaticExternalSpaStoreTask(IConfiguration config, IRemoteExternalSPAStore store, IHealthCheckStore healthCheckStore)
        {
            RemoteExternalSPAStore = store;
            HealthCheckStore = healthCheckStore;
            _config = config;
        }
        public string Schedule => "*/1 * * * *";  // every 1 minute
        private async Task<RemoteUrls> GetRemoteUrlsAsync(string url, bool validateSchema = false)
        {
            try
            {
                var schemaUrl = url.Replace(".json", ".schema.json");
                var schema = await RemoteJsonFetch.GetRemoteJsonContentAsync(schemaUrl);

                string content = await RemoteJsonFetch.GetRemoteJsonContentAsync(url, schema);
                RemoteUrls remoteUrls;
                remoteUrls = ExternUrlsOptionConvert.FromJson(content);

                return remoteUrls;
            }
            catch (Exception e)
            {
                logger.Fatal("Exception Caught:{0}", e.Message);
            }
            return null;
        }
        public async Task Invoke(CancellationToken cancellationToken)
        {
            var key = "remote-static-esternal-spa";
            var currentHealth = await HealthCheckStore.GetHealthAsync(key);
            if (currentHealth == null)
            {
                currentHealth = new HealthRecord()
                {
                    Healty = false,
                    State = null
                };
                await HealthCheckStore.SetHealthAsync(key, currentHealth);
            }

            var appConfig = new ExternalUrlsOptions();
            _config.GetSection("externalSPAConfigs").Bind(appConfig);

            var urlViewSchema = await RemoteJsonFetch.GetRemoteJsonContentAsync(appConfig.UrlViewSchema);

            var remoteViewUrls = await GetRemoteUrlsAsync(appConfig.Urls, true);
            var loaded = false;
            foreach (var url in remoteViewUrls.Urls)
            {
                loaded = await RemoteExternalSPAStore.LoadRemoteDataAsync(url);
                if (!loaded)
                {
                    logger.Fatal("Failed to load:{0}",url);
                    break;
                }
            }
            if (loaded && !currentHealth.Healty)
            {
                // once loaded we are good
                currentHealth = new HealthRecord()
                {
                    Healty = true,
                    State = null
                };
                await HealthCheckStore.SetHealthAsync(key, currentHealth);
            }
        }
    }
}