using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hugo.Data.Json;
using IdentityServer4.Models;
using P7.Core.Linq;
using P7.HugoStore.Core;
using P7.IdentityServer4.Common;
using P7.Store;

namespace P7.IdentityServer4.BiggyStore
{
    public class ApiResourceStore : HugoStoreBase<ApiResourceDocument>, IApiResourceStore
    {
        public ApiResourceStore(IIdentityServer4BiggyConfiguration biggyConfiguration) :
            base(biggyConfiguration, "api_resource")
        {
        }

        public async Task InsertApiResourceAsync(ApiResource apiResource)
        {
            var doc = new ApiResourceDocument(apiResource);
            await InsertAsync(doc);
        }

        public async Task DeleteApiResourceByNameAsync(string name)
        {
            var doc = new ApiResourceDocument(new ApiResource()
            {
                Name = name
            });
            await DeleteAsync(doc.TenantId_G,doc.Id_G);
        }

        public async Task<IPage<ApiResource>> PageAsync(int pageSize, byte[] pagingState)
        {
            byte[] currentPagingState = pagingState;
            PagingState ps = pagingState.DeserializePageState();
            var records = await RetrieveAsync();
            records = records.OrderBy(o => o.Name).ToList();

            var predicate = PredicateBuilder.True<ApiResourceDocument>();

            var filtered = records.Where(predicate.Compile()).Select(i => i);

            var slice = filtered.Skip(ps.CurrentIndex).Take(pageSize).ToList();
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

            List<ApiResource> apiResourceSlice = new List<ApiResource>();
            foreach (var item in slice)
            {
                var client = await item.MakeApiResourceAsync();
                apiResourceSlice.Add(client);
            }

            var page = new PageProxy<ApiResource>(currentPagingState, pagingState, apiResourceSlice);
            return page;
        }
    }
}