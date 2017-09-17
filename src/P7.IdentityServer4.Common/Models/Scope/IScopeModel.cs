using System.Threading.Tasks;

namespace P7.IdentityServer4.Common
{
    public interface IScopeModel
    {
        Task<global::IdentityServer4.Models.Scope> MakeScopeAsync();
    }
}