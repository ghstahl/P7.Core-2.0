using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace P7.Razor.FileProvider
{
    public class RemoteFileProvider : IFileProvider, IDisposable
    {
        private readonly IDistributedCache _cache = null;
        public RemoteFileProvider(IDistributedCache cache)
        {
            _cache = cache;
        }
        public IFileInfo GetFileInfo(string subpath)
        {
            throw new NotImplementedException();
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new NotFoundDirectoryContents();
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
