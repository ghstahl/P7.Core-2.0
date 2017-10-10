using System.Threading.Tasks;
using P7.IdentityServer4.AspNetIdentity.Configuration;

namespace P7.IdentityServer4.AspNetIdentity.Stores
{
    public interface IRemoteIdentityServerDiscoveryStore : IIdentityServerDiscoveryStore
    {
        Task<bool> LoadRemoteDataAsync(string url);
    }
}