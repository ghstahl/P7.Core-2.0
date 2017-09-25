using System.Collections.Generic;
using System.Threading.Tasks;

namespace P7.SessionContextStore.Core
{
    public class SessionContext : ISessionContext
    {
        Dictionary<string,object> Values = new Dictionary<string, object>();
        private IRemoteSessionContext _remoteSessionContext;
        private string ContextKey { get; set; }
        public SessionContext(IRemoteSessionContext remoteSessionContext)
        {
            _remoteSessionContext = remoteSessionContext;
        }

        public void SetContextKey(string contextKey)
        {
            ContextKey = contextKey;
            _remoteSessionContext.SetContextKey(ContextKey);

        }

        public async Task<object> GetValueAsync<T>(string key) where T : class
        {
            object value;
            if (Values.TryGetValue(key, out value))
            {
                return (T) value;
            }
            value = await _remoteSessionContext.GetValueAsync<T>(key);
            Values.Add(key,value);
            return (T)value;
        }
    }
}