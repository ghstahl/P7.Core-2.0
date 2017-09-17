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
    public class IdentityResourceStore : HugoStoreBase<IdentityResourceDocument>, IIdentityResourceStore
    {
        public IdentityResourceStore(IIdentityServer4BiggyConfiguration biggyConfiguration) :
            base(biggyConfiguration, "identity_resource")
        {
        }

        public async Task InsertIdentityResourceAsync(IdentityResource identityResource)
        {
            var doc = new IdentityResourceDocument(identityResource);
            await InsertAsync(doc);
        }

        public async Task DeleteIdentityResourceByNameAsync(string name)
        {
            var doc = new IdentityResourceDocument(new IdentityResource()
            {
                Name = name
            });
            await DeleteAsync(doc.TenantId_G, doc.Id_G);
        }

        public async Task<IPage<IdentityResource>> PageAsync(int pageSize, byte[] pagingState)
        {
            byte[] currentPagingState = pagingState;
            PagingState ps = pagingState.DeserializePageState();
            var records = await RetrieveAsync();
            records = records.OrderBy(o => o.Name).ToList();

            var predicate = PredicateBuilder.True<IdentityResourceDocument>();

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

            List<IdentityResource> identityResourceSlice = new List<IdentityResource>();
            foreach (var item in slice)
            {
                var client = await item.MakeIdentityResourceAsync();
                identityResourceSlice.Add(client);
            }

            var page = new PageProxy<IdentityResource>(currentPagingState, pagingState, identityResourceSlice);
            return page;
        }
    }
}