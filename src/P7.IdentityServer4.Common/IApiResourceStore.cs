using System.Threading.Tasks;
using IdentityServer4.Models;
using P7.Store;

namespace P7.IdentityServer4.Common
{
    public interface IApiResourceStore
    {
        Task InsertApiResourceAsync(ApiResource apiResource);

        Task DeleteApiResourceByNameAsync(string name);

        Task<IPage<ApiResource>> PageAsync(int pageSize,
            byte[] pagingState);
    }
}