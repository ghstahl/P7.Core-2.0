using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hugo.Data.Json;
using P7.Core.Utils;
using P7.Store;

namespace P7.HugoStore.Core
{
    public class HugoStoreBase<T> where T : class, IDocumentBaseWithTenant, new()
    {
        public delegate bool ContainsAnyInList<T>(List<T> a, List<T> b);

        private ContainsAnyInList<string> _containsAnyInList;

        public ContainsAnyInList<string> DelegateContainsAnyInStringList
        {
            get
            {
                if (_containsAnyInList == null)
                {
                    _containsAnyInList = (a, b) =>
                    {
                        var result = a.Any(x => b.Contains(x));
                        return result;
                    };
                }
                return _containsAnyInList;
            }
        }

        protected IBiggyConfiguration _biggyConfiguration;

        protected HugoStoreBase(IBiggyConfiguration biggyConfiguration, string collection)
        {
            _biggyConfiguration = biggyConfiguration;
            _collection = collection;
        }
        protected static object TheLock
        {
            get { return ConcurrencyLock.TheLock; }
        }

        protected P7JsonStore<T> _theStore = null;
        private string _collection;

        protected P7JsonStore<T> Store
        {
            get
            {
                if (_theStore == null)
                {
                    _theStore =
                        new P7JsonStore<T>(_biggyConfiguration.FolderStorage,
                            _biggyConfiguration.DatabaseName, _collection);
    //                _theStore.Sorter = _sorter;
                }
                return _theStore;
            }
        }

        protected async Task GoAsync(Action action)
        {
            await Task.Run(action);
        }

        protected static async Task<TResult> GoAsync<TResult>(Func<TResult> func)
        {

            var task = Task.Run(func);
            var result = await task;
            return result;

        }

        public async Task InsertAsync(T doc)
        {
            var existing = await FetchAsync(doc.TenantId_G,doc.Id_G);
            await GoAsync(() =>
            {
                lock (TheLock)
                {

                    if (existing == null)
                    {
                        Store.Add(doc);
                    }
                    else
                    {
                        Store.Update(doc);
                    }
                }
            });
        }

        public async Task<T> FetchAsync(Guid tenantId,Guid id)
        {
            var result = await GoAsync(() =>
                {
                    T r2 = null;
                    lock (TheLock)
                    {
                        var collection = this.Store.TryLoadData();
                        var query = from item in collection
                            where item.TenantId_G == tenantId && item.Id_G == id
                                    select item;
                        if (!query.Any())
                            return null;
                        var record = query.SingleOrDefault();

                        r2 = record;
                    }
                    return r2;
                }
            );
            return result;
        }

        public async Task UpdateAsync(T doc)
        {
            await InsertAsync(doc);
        }

        public async Task DeleteAsync(Guid tenantId, Guid id)
        {
            await GoAsync(() =>
            {
                lock (TheLock)
                {
                    var collection = this.Store.TryLoadData();
                    var query = from item in collection
                        where item.TenantId_G == tenantId && item.Id_G == id
                        select item;
                    foreach (var item in query)
                    {
                        this.Store.Delete(item);
                    }
                }
            });
        }

        private async Task<List<T>> RetrieveAllAsync()
        {

            var result = await GoAsync(() =>
                {
                    List<T> r2 = null;
                    lock (TheLock)
                    {
                        var collection = this.Store.TryLoadData();
                        if (collection.Any())
                        {
                            r2 = collection.ToList();
                        }
                    }
                    return r2 ?? new List<T>();
                }
            );
            return result;
        }
        public async Task<List<T>> RetrieveAsync(Guid? tenantId = null)
        {

            if (tenantId == null)
                return await RetrieveAllAsync();
            var result = await GoAsync(() =>
            {
                List<T> r2 = null;
                lock (TheLock)
                {
                    var collection = this.Store.TryLoadData();
                    var query = from item in collection
                                where item.TenantId_G == tenantId
                                select item;

                    if (query.Any())
                    {
                        r2 = query.ToList();
                    }
                }
                return r2 ?? new List<T>();
            }
            );
            return result;
        }
        public async Task<IPage<T>> PageAsync(
            int pageSize,
            byte[] pagingState,
            Guid? tenantId = null)
        {
            byte[] currentPagingState = pagingState;
            PagingState ps = pagingState.DeserializePageState();
            var records = await RetrieveAsync(tenantId);

            var slice = records.Skip(ps.CurrentIndex).Take(pageSize).ToList();
            if (slice.Count < pageSize)
            {
                // we are at the end
                pagingState = null;
            }
            else
            {
                ps.CurrentIndex += pageSize;
                pagingState = ps.Serialize();
            }

            var page = new PageProxy<T>(currentPagingState, pagingState, slice);
            return page;
        }

    }
}