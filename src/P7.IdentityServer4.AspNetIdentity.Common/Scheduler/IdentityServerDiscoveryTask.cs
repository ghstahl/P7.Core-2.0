using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using P7.Core.Scheduler.Scheduling;
using P7.HealthCheck.Core;
using P7.IdentityServer4.AspNetIdentity.Configuration;
using P7.IdentityServer4.AspNetIdentity.Stores;

namespace P7.IdentityServer4.AspNetIdentity.Scheduler
{
    public class IdentityServerDiscoveryTask : IScheduledTask
    {
        private IOptions<IdentityServerConfig> Optons { get; set; }
        private IRemoteIdentityServerDiscoveryStore RemoteIdentityServerDiscoveryStore { get; set; }
        private IHealthCheckStore HealthCheckStore { get; set; }
        public IdentityServerDiscoveryTask(
            IOptions<IdentityServerConfig> options,
            IRemoteIdentityServerDiscoveryStore remoteIdentityServerDiscoveryStore, 
            IHealthCheckStore healthCheckStore)
        {
            Optons = options;
            RemoteIdentityServerDiscoveryStore = remoteIdentityServerDiscoveryStore;
            HealthCheckStore = healthCheckStore;
        }
        public string Schedule => "*/1 * * * *";  // every 1 minute

        public async Task Invoke(CancellationToken cancellationToken)
        {
            var key = "identityserver-discovery";
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
            var url = Optons.Value.Discovery;
            var loaded = await RemoteIdentityServerDiscoveryStore.LoadRemoteDataAsync(url);
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