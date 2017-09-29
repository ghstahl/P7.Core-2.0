using System.Threading;
using System.Threading.Tasks;
using P7.Core.Scheduler.Scheduling;
using P7.External.SPA.Core;

namespace P7.External.SPA.Scheduler
{
    public class RemoteStaticExternalSpaStoreTask : IScheduledTask
    {
        private const string Url = "https://rawgit.com/ghstahl/P7.Core-2.0/master/src/WebApplication1/external.spa.config.json";
        private IRemoteExternalSPAStore RemoteExternalSPAStore { get; set; }
        public RemoteStaticExternalSpaStoreTask(IRemoteExternalSPAStore store)
        {
            RemoteExternalSPAStore = store;
        }
        public string Schedule => "*/1 * * * *";  // every 1 minute

        public async Task Invoke(CancellationToken cancellationToken)
        {
            await RemoteExternalSPAStore.LoadRemoteDataAsync(Url);
        }
    }
}