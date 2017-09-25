using System.Threading.Tasks;

namespace P7.SessionContextStore.Core
{
    public interface ISessionContextStore
    {
        Task<ISessionContext> GetSessionContextAsync();
    }
}