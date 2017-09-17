using System.Threading.Tasks;
using IdentityServer4.Models;
using P7.Store;

namespace P7.IdentityServer4.Common
{
    public interface IIdentityResourceStore
    {
        Task InsertIdentityResourceAsync(IdentityResource identityResource);

        Task DeleteIdentityResourceByNameAsync(string name);

        Task<IPage<IdentityResource>> PageAsync(int pageSize,
            byte[] pagingState);
    }
}