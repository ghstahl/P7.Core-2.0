using System.Threading.Tasks;

namespace P7.External.SPA.Core
{
    public interface IRemoteExternalSPAStore : IExternalSPAStore
    {
        Task LoadRemoteDataAsync(string url);
    }
}