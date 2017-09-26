using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace P7.SessionContextStore.Core
{
    public interface IRemoteSessionContextAccessor
    {
        Task<string> GetCurrentContextKeyAsync();
        Task<bool> GetContextKeyExistsAsync(string contextKey);

        Task<IRemoteSessionContext> GetRemoteSessionContextAsync<T>(string contextKey);
    }

}