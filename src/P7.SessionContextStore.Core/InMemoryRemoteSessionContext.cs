using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;

namespace P7.SessionContextStore.Core
{
    public interface IInMemoryRemoteSessionContext
    {
        void Add<T>(string contextKey, string key, T value);
        void Remove(string contextKey, string key);
        void RemoveContextKey(string contextKey);
        void RemoveAll();
        void SetCurrentContext(string current);
    }
    public class InMemoryRemoteSessionContext : IRemoteSessionContext, IInMemoryRemoteSessionContext
    {
        private Dictionary<string, Dictionary<string, object>> Values { get; set; }
        private string _contextKey;
        private object MyLock = new object();
        public InMemoryRemoteSessionContext()
        {
            Values = new Dictionary<string, Dictionary<string, object>>();
        }

        public void SetContextKey(string contextKey)
        {
            _contextKey = contextKey;
        }

        public async Task<object> GetValueAsync<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(_contextKey))
            {
                throw new Exception("You need to set the contextKey");
            }
            Dictionary<string, object> dictToDict;
            if (Values.TryGetValue(_contextKey, out dictToDict))
            {
                object value;
                if (dictToDict.TryGetValue(_contextKey, out value))
                {
                    return value as T;
                }
            }
            return null;
        }

        public void Add<T>(string contextKey, string key, T value)
        {
            lock (MyLock)
            {
                if (!Values.ContainsKey(contextKey))
                {
                    Values.Add(contextKey,new Dictionary<string, object>());
                }
                var context = Values[contextKey];
                context.Add(key,value);
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
            }
        }
    }
}