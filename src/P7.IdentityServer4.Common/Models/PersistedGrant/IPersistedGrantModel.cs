using System.Threading.Tasks;

namespace P7.IdentityServer4.Common
{
    public interface IPersistedGrantModel
    {
        Task<global::IdentityServer4.Models.PersistedGrant> MakePersistedGrantAsync();
    }
}