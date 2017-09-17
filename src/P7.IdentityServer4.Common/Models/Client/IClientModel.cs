using System.Threading.Tasks;

namespace P7.IdentityServer4.Common
{
    public interface IClientModel
    {
        Task<global::IdentityServer4.Models.Client> MakeClientAsync();
        string ClientId { get; set; }
    }
}