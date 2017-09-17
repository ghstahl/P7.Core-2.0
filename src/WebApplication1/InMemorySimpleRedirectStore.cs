using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P7.SimpleRedirect.Core;

namespace WebApplication1
{
    class InMemorySimpleRedirectStore : ISimpleRedirectorStore
    {
        private List<SimpleRedirectRecord> _simpleRedirectRecords;

        private List<SimpleRedirectRecord> Records
        {
            get
            {
                if (_simpleRedirectRecords == null)
                {
                    _simpleRedirectRecords = new List<SimpleRedirectRecord>()
                    {
                        new SimpleRedirectRecord() {BaseUrl = "www.google.com", Key = "google", Scheme = "https"},
                        new SimpleRedirectRecord() {BaseUrl = "www.facebook.com", Key = "facebook", Scheme = "https"},
                        new SimpleRedirectRecord() {BaseUrl = "www.microsoft.com", Key = "microsoft", Scheme = "https"}
                    };
                }
                return _simpleRedirectRecords;
            }
        }

        public async Task<SimpleRedirectRecord> FetchRedirectRecord(string key)
        {
            var query = from item in Records
                where item.Key == key
                select item;
            return Enumerable.FirstOrDefault<SimpleRedirectRecord>(query);
        }
    }
}