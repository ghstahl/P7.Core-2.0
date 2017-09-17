using System.Threading.Tasks;

namespace P7.IdentityServer4.Common
{
    public interface IApiResourceModel
    {
        Task<global::IdentityServer4.Models.ApiResource> MakeApiResourceAsync();
    }
}