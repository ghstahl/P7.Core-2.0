using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using P7.HugoStore.Core;
using P7.IdentityServer4.Common;

namespace P7.IdentityServer4.BiggyStore
{
    public class PersistedGrantStore : HugoStoreBase<PersistedGrantDocument>, IPersistedGrantStore
    {
        public PersistedGrantStore(IIdentityServer4BiggyConfiguration biggyConfiguration) :
            base(biggyConfiguration, "persisted_grant")
        {
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            var doc = new PersistedGrantDocument(grant);
            await InsertAsync(doc);
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var grant = new PersistedGrant()
            {
                Key = key
            };
            var doc = new PersistedGrantDocument(grant);
            var result = await FetchAsync(doc.TenantId_G, doc.Id_G);
            if (result == null)
                return null;
            return await result.ToPersistedGrantAsync();
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var result = await RetrieveAsync();
            var query = from item in result
                where item.SubjectId == subjectId
                select item.ToPersistedGrant();
            return query;
        }

        public async Task RemoveAsync(string key)
        {
            var doc = new PersistedGrantDocument(new PersistedGrant()
            {
                Key = key
            });
            await DeleteAsync(doc.TenantId_G, doc.Id_G);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            var result = await RetrieveAsync();
            var query = from item in result
                where item.SubjectId == subjectId && item.ClientId == clientId
                select item;
            var final = query.ToList();
            foreach (var doc in final)
            {
                await DeleteAsync(doc.TenantId_G, doc.Id_G);
            }
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var result = await RetrieveAsync();
            var query = from item in result
                where item.SubjectId == subjectId && item.ClientId == clientId && item.Type == type
                select item;
            var final = query.ToList();
            foreach (var doc in final)
            {
                await DeleteAsync(doc.TenantId_G, doc.Id_G);
            }
        }
    }
}