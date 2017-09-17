using System.Threading.Tasks;

namespace P7.IdentityServer4.Common
{
    public interface IIdentityResourceModel
    {
        Task<global::IdentityServer4.Models.IdentityResource> MakeIdentityResourceAsync();
    }
}