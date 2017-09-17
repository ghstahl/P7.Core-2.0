using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P7.HugoStore.Core;
using P7.Store;

namespace P7.SimpleDocument.Store.Hugo
{
    public class HugoSimpleDocumentStore<T> : HugoStoreBase<SimpleDocument<T>>, ISimpleDocumentStoreWithTenant<T>
        where T : class, IComparable, new()
    {
        private string TenantId { get; set; }
        public HugoSimpleDocumentStore(IBiggyConfiguration biggyConfiguration, string collection)
            : base(biggyConfiguration, collection)
        {
            TenantId = biggyConfiguration.TenantId;
        }

        private static List<SimpleDocument<T>> Filter(List<SimpleDocument<T>> collection, MetaData metaData)
        {
            if (metaData == null)
                return collection;

            if (string.IsNullOrEmpty(metaData.Category))
                return collection;
            if (string.IsNullOrEmpty(metaData.Version))
            {
                var query = from item in collection
                    where item.MetaData.Category == metaData.Category
                    select item;
                return query.ToList();
            }

            var queryFull = from item in collection
                where item.MetaData.Category == metaData.Category && item.MetaData.Version == metaData.Version
                select item;
            return queryFull.ToList();
        }

        public async Task<IPage<SimpleDocument<T>>> PageAsync(int pageSize, byte[] pagingState, Guid? tenantId = null,
            MetaData metaData = null)
        {
            if (metaData == null)
                return await base.PageAsync(pageSize, pagingState, tenantId);

            byte[] currentPagingState = pagingState;
            PagingState ps = pagingState.DeserializePageState();
            var records = await RetrieveAsync(tenantId);
            records = Filter(records, metaData);
            var slice = records.Skip(ps.CurrentIndex).Take(pageSize).ToList();
            if (slice.Count < pageSize)
            {
                // we are at the end
                pagingState = null;
            }
            else
            {
                ps.CurrentIndex += pageSize;
                pagingState = ps.Serialize();
            }

            var page = new PageProxy<SimpleDocument<T>>(currentPagingState, pagingState, slice);
            return page;
        }

        public Task<string> GetTenantIdAsync()
        {
            return Task.FromResult(TenantId);
        }

        public async Task<ICollection<SimpleDocument<T>>> PageAsync(int pageSize, int page, Guid? tenantId = null, MetaData metaData = null)
        {
            PagingState ps = new PagingState() {CurrentIndex = pageSize * (page - 1)};
            var pagingState = ps.Serialize();
            return await PageAsync(pageSize, pagingState, tenantId, metaData);
        }
    }
}
