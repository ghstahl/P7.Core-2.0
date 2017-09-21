using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using P7.Core.Scheduler.Scheduling;
using P7.RazorProvider.Store.Core.Interfaces;

namespace P7.External.SPA.Scheduler
{
    public class RemoteRazorLocationStoreTask : IScheduledTask
    {
        private const string Url = "https://rawgit.com/ghstahl/P7/master/src/p7.external.spa/Areas/ExtSpa/views.json";
        private IRemoteRazorLocationStore RemoteRazorLocationStore { get; set; }
        public RemoteRazorLocationStoreTask(IRemoteRazorLocationStore store)
        {
            RemoteRazorLocationStore = store;
        }
        public string Schedule => "*/1 * * * *";  // every 1 minute

        public async Task Invoke(CancellationToken cancellationToken)
        {
            await RemoteRazorLocationStore.LoadRemoteDataAsync(Url);
        }
    }
}
