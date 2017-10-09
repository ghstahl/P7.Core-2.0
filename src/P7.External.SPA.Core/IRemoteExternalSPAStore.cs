using System.Threading.Tasks;

namespace P7.External.SPA.Core
{
    public interface IRemoteExternalSPAStore : IExternalSPAStore
    {
        Task<bool> LoadRemoteDataAsync(string url);
    }
}