using System.Threading.Tasks;

namespace P7.SessionContextStore.Core
{
    public interface ISessionContext
    {
        void SetContextKey(string contextKey);
        Task<object> GetValueAsync<T>(string key) where T : class;

    }
}