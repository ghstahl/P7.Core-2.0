using System.Threading.Tasks;

namespace P7.IdentityServer4.Common
{
    public interface IConsentModel
    {
        Task<global::IdentityServer4.Models.Consent> MakeConsentAsync();
    }
}