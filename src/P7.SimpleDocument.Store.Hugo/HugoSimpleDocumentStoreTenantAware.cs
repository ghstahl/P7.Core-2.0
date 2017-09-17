using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P7.HugoStore.Core;
using P7.Store;

namespace P7.SimpleDocument.Store.Hugo
{
    public class HugoSimpleDocumentStoreTenantAware<T> : HugoSimpleDocumentStore<T>, ISimpleDocumentStore<T>
        where T : class, IComparable, new()
    {
        private Guid _tenantId;

        public HugoSimpleDocumentStoreTenantAware(IBiggyConfiguration biggyConfiguration,
            string collection) :
            base(biggyConfiguration, collection)
        {
            _tenantId = Guid.Parse(biggyConfiguration.TenantId);
        }

        public async Task DeleteAsync(Guid id)
        {
            await base.DeleteAsync(_tenantId, id);
        }

        public async Task<SimpleDocument<T>> FetchAsync(Guid id)
        {
            return await base.FetchAsync(_tenantId, id);
        }

        public async Task<IPage<SimpleDocument<T>>> PageAsync(int pageSize, byte[] pagingState, MetaData metaData = null)
        {
            return await base.PageAsync(pageSize, pagingState, _tenantId, metaData);
        }

        public async Task<ICollection<SimpleDocument<T>>> PageAsync(int pageSize, int page, MetaData metaData = null)
        {
            PagingState ps = new PagingState() {CurrentIndex = pageSize * (page - 1)};
            var pagingState = ps.Serialize();
            return await PageAsync(pageSize, pagingState, metaData);
        }
    }
}