using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;

namespace P7.SessionContextStore.Core
{

    public interface IInMemoryRemoteSessionContextAccessor
    {
        void Add<T>(string contextKey, string key, T value);
        void Remove(string contextKey, string key);
        void RemoveContextKey(string contextKey);
        void RemoveAll();
        void SetCurrentContext(string current);
        object GetValue<T>(string contextKey, string key) where T : class;

    }


    public class InMemoryRemoteSessionContextAccessor :
        IInMemoryRemoteSessionContextAccessor,
        IRemoteSessionContextAccessor
    {
        private Dictionary<string, Dictionary<string, object>> Values { get; set; }
        private string _currentContextKey;
        private object MyLock = new object();

        public InMemoryRemoteSessionContextAccessor()
        {
            Values = new Dictionary<string, Dictionary<string, object>>();
        }
        public void Add<T>(string contextKey, string key, T value)
        {
            lock (MyLock)
            {
                if (!Values.ContainsKey(contextKey))
                {
                    Values.Add(contextKey, new Dictionary<string, object>());
                }
                var context = Values[contextKey];
                context.Add(key, value);
            }
        }

        public void Remove(string contextKey, string key)
        {
            lock (MyLock)
            {
                if (Values.ContainsKey(contextKey))
                {
                    var context = Values[contextKey];
                    if (context.ContainsKey(key))
                    {
                        context.Remove(key);
                    }
                }
            }
        }

        public void RemoveContextKey(string contextKey)
        {
            lock (MyLock)
            {
                if (Values.ContainsKey(contextKey))
                {
                    Values.Remove(contextKey);
                }
            }
        }

        public void RemoveAll()
        {
            lock (MyLock)
            {
                Values = new Dictionary<string, Dictionary<string, object>>();
            }
        }

        public void SetCurrentContext(string current)
        {
            lock (MyLock)
            {
                if (!Values.ContainsKey(current))
                {
                    throw new Exception($"{current} does not exist!");
                }
                _currentContextKey = current;
            }
        }

        public object GetValue<T>(string contextKey, string key) where T : class
        {
            lock (MyLock)
            {
                if (!Values.ContainsKey(contextKey))
                {
                    throw new Exception($"{contextKey} does not exist!");
                }
                if (!Values[contextKey].ContainsKey(key))
                {
                    throw new Exception($"{contextKey}|{key} does not exist!");
                }
                return Values[contextKey][key] as T;
            }
        }

        public async Task<string> GetCurrentContextKeyAsync()
        {
            lock (MyLock)
            {
                if (string.IsNullOrEmpty(_currentContextKey))
                {
                    if (Values.Count > 0)
                    {
                        _currentContextKey = Values.First().Key;
                    }
                }
                return _currentContextKey;
            }
        }

        public async Task<bool> GetContextKeyExistsAsync(string contextKey)
        {
            lock (MyLock)
            {
                return Values.ContainsKey(contextKey);
            }
        }

        public async Task<IRemoteSessionContext> GetRemoteSessionContextAsync<T>(string contextKey)
        {
            lock (MyLock)
            {
                if (!string.IsNullOrEmpty(contextKey))
                {
                    if (Values.ContainsKey(contextKey))
                    {
                        var context = new InMemoryRemoteSessionContext(this);
                        context.SetContextKey(contextKey);
                        return context;
                    }
                }
                throw new Exception($"{contextKey} does not exist!");   
            }
        }
    }

    public class InMemoryRemoteSessionContext : 
        IRemoteSessionContext
    {
        private InMemoryRemoteSessionContextAccessor _accessor;
        private string _contextKey;
        public InMemoryRemoteSessionContext(InMemoryRemoteSessionContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public void SetContextKey(string contextKey)
        {
            _contextKey = contextKey;
        }

        public async Task<object> GetValueAsync<T>(string key) where T : class
        {
            return _accessor.GetValue<T>(_contextKey, key);
        }
    }
}