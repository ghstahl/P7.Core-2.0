using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P7.Core.Scheduler.Scheduling;
using P7.RazorProvider.Store.Core.Interfaces;
using P7.SessionContextStore.Core;
using WebApplication1.Models;

namespace WebApplication1.Scheduler
{
   
    public class InMemoryRemoteSessionContextTask : IScheduledTask
    {
        private IInMemoryRemoteSessionContext InMemoryRemoteSessionContext { get; set; }

        public InMemoryRemoteSessionContextTask(IInMemoryRemoteSessionContext inMemoryRemoteSessionContext)
        {
            InMemoryRemoteSessionContext = inMemoryRemoteSessionContext;
        }

        public string Schedule => "0 0 */1 * *"; // every 1 day

        public async Task Invoke(CancellationToken cancellationToken)
        {
            InMemoryRemoteSessionContext.Add("version1", "a", "a value");
            InMemoryRemoteSessionContext.Add("version1", "b", "b value");
            InMemoryRemoteSessionContext.Add("version1", "c", "c value");
            InMemoryRemoteSessionContext.Add("version1", "some-data",
                new SomeData {IsWild = true, RequestId = "Hi There"});

            InMemoryRemoteSessionContext.Add("version2", "d", "d value");
            InMemoryRemoteSessionContext.Add("version2", "e", "e value");
            InMemoryRemoteSessionContext.Add("version2", "f", "f value");
            InMemoryRemoteSessionContext.Add("version2", "some-data",
                new SomeData {IsWild = false, RequestId = "Hi There 2"});

            InMemoryRemoteSessionContext.SetCurrentContext("version2");
        }
    }
}
