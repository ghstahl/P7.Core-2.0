using System;
using Microsoft.Extensions.Primitives;
using P7.RazorProvider.Store.Core.Interfaces;
using P7.RazorProvider.Store.Core.Models;

namespace P7.Razor.FileProvider
{
    public class RazorChangeToken : IChangeToken
    {
        private string _viewPath;
        private IRazorLocationStore _store;
        public RazorChangeToken(IRazorLocationStore store, string viewPath)
        {
            _store = store;
            _viewPath = viewPath;
        }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state) => EmptyDisposable.Instance;


        public bool HasChanged
        {
            get
            {
                var query = new RazorLocationQuery() {Location = _viewPath};
                

                var doc = _store.FetchAsync(query).GetAwaiter().GetResult();
                if (doc != null)
                {
                    return doc.LastModified > doc.LastRequested;
                }
                return false;
            }
        }

        public bool ActiveChangeCallbacks => false;
    }
}