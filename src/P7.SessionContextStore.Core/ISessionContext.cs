using System.Threading.Tasks;

namespace P7.SessionContextStore.Core
{
    public interface ISessionContext
    {
        void SetContextKey(string contextKey);
        Task<T> GetValueAsync<T>(string key);

    }
}