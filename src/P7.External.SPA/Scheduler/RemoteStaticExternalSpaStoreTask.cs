using System.Threading;
using System.Threading.Tasks;
using P7.Core.Scheduler.Scheduling;
using P7.External.SPA.Core;
using P7.HealthCheck.Core;

namespace P7.External.SPA.Scheduler
{
    public class RemoteStaticExternalSpaStoreTask : IScheduledTask
    {
        private const string Url = "https://rawgit.com/ghstahl/P7.Core-2.0/master/src/WebApplication1/external.spa.config.json";
        private IRemoteExternalSPAStore RemoteExternalSPAStore { get; set; }
        private IHealthCheckStore HealthCheckStore { get; set; }
        public RemoteStaticExternalSpaStoreTask(IRemoteExternalSPAStore store, IHealthCheckStore healthCheckStore)
        {
            RemoteExternalSPAStore = store;
            HealthCheckStore = healthCheckStore;
        }
        public string Schedule => "*/1 * * * *";  // every 1 minute

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

            var loaded = await RemoteExternalSPAStore.LoadRemoteDataAsync(Url);
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