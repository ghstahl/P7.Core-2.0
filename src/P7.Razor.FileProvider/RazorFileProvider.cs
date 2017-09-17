using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using P7.RazorProvider.Store.Core.Interfaces;

namespace P7.Razor.FileProvider
{
    public class RazorFileProvider : IFileProvider
    {
        private IRazorLocationStore _store;

        public RazorFileProvider(IRazorLocationStore store)
        {
            _store = store;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var result = new RazorFileInfo(_store, subpath);
            result.GetView().GetAwaiter().GetResult();
            IFileInfo r = null;
            if (result.Exists)
            {
                r = result;
            }
            else
            {
                r = new NotFoundFileInfo(subpath);
            }
            return r;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new NotFoundDirectoryContents();
        }

        public IChangeToken Watch(string filter)
        {
            return new RazorChangeToken(_store, filter);
        }
    }
}