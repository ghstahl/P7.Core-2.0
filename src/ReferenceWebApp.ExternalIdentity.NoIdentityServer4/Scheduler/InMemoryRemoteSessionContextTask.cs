using System.Threading;
using System.Threading.Tasks;
using P7.Core.Scheduler.Scheduling;
using P7.SessionContextStore.Core;
using ReferenceWebApp.Models;

namespace ReferenceWebApp.Scheduler
{
   
    public class InMemoryRemoteSessionContextTask : IScheduledTask
    {
        private InMemoryRemoteSessionContextAccessor Accessor { get; set; }

        public InMemoryRemoteSessionContextTask(InMemoryRemoteSessionContextAccessor accessor)
        {
            Accessor = accessor;
        }

        public string Schedule => "0 0 */1 * *"; // every 1 day

        public async Task Invoke(CancellationToken cancellationToken)
        {
            Accessor.Add("version1", "a", "a value");
            Accessor.Add("version1", "b", "b value");
            Accessor.Add("version1", "c", "c value");
            Accessor.Add("version1", "some-data",
                new SomeData {IsWild = true, RequestId = "Hi There"});

            Accessor.Add("version2", "d", "d value");
            Accessor.Add("version2", "e", "e value");
            Accessor.Add("version2", "f", "f value");
            Accessor.Add("version2", "some-data",
                new SomeData {IsWild = false, RequestId = "Hi There 2"});

        }
    }
}
