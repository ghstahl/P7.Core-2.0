using System.Threading.Tasks;

namespace P7.SessionContextStore.Core
{
    public interface ISessionContext
    {
        Task<object> GetValueAsync<T>(string key) where T : class;
    }
    public interface ISessionContextPrivate
    {
        void SetContextKey(string contextKey);
    }

    
}