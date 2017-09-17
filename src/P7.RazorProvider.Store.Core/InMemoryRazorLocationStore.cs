using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P7.RazorProvider.Store.Core.Interfaces;
using P7.RazorProvider.Store.Core.Models;
using P7.Store;

namespace P7.RazorProvider.Store.Core
{
    public class InMemoryRazorLocationStore : IRazorLocationStore
    {
        private Dictionary<string, RazorLocation> _records;

        protected Dictionary<string, RazorLocation> Records
        {
            get => _records ?? (_records = new Dictionary<string, RazorLocation>());
            set => _records = value;
        }
        public async Task InsertAsync(RazorLocation document)
        {
            lock (Records)
            {
                Records[document.Location] = document;
            }
        }
        public void Insert(RazorLocation document)
        {
            lock (Records)
            {
                Records[document.Location] = document;
            }
        }
        public void Insert(List<RazorLocation> documents)
        {
            lock (Records)
            {
                foreach (var doc in documents)
                {
                    Records[doc.Location] = doc;
                }
            }
        }
        public async Task UpdateAsync(RazorLocation document)
        {
            lock (Records)
            {
                Records[document.Location] = document;
            }
        }

        public async Task DeleteAsync(RazorLocationQuery query)
        {
            lock (Records)
            {
                if (Records.ContainsKey(query.Location))
                {
                    Records.Remove(query.Location);
                }
            }
        }

        public async Task<RazorLocation> FetchAsync(RazorLocationQuery query)
        {
            lock (Records)
            {
                if (Records.ContainsKey(query.Location))
                {
                    return Records[query.Location];
                }
                return null;
            }
        }

        public async Task<IPage<RazorLocation>> PageAsync(int pageSize, byte[] pagingState)
        {
            lock (Records)
            {
                var pageState = pagingState.DeserializePageState();


                var a = Records.Skip(pageState.CurrentIndex).Take(pageSize).ToList();
                byte[] nextPagingState = null;
                if (a.Count == pageSize)
                {
                    pageState.CurrentIndex += pageSize;
                    nextPagingState = pageState.Serialize();
                }
                var query = from item in a
                    let c = item.Value
                    select c;
                var pageProxy = new PageProxy<RazorLocation>(pagingState, nextPagingState, query.ToList());
                return pageProxy;
            }
        }
    }
}
