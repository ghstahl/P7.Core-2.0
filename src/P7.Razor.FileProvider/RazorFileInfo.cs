using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.FileProviders;
using P7.Core.Cache;
using P7.Core.Reflection;
using P7.Core.Utils;
using P7.RazorProvider.Store.Core.Interfaces;
using P7.RazorProvider.Store.Core.Models;


namespace P7.Razor.FileProvider
{
    public class RazorFileInfo : IFileInfo
    {
        private readonly IDistributedCache _cache = null;
        private IRazorLocationStore _store;
        private string _viewPath;
        private byte[] _viewContent;
        private DateTimeOffset _lastModified;
        private bool _exists;

        public RazorFileInfo(IDistributedCache cache, IRazorLocationStore store, string viewPath)
        {
            _cache = cache;
            _viewPath = viewPath;
            _store = store;
        }

        public Stream CreateReadStream()
        {
            return new MemoryStream(_viewContent);
        }

        public bool Exists => _exists;

        public long Length
        {
            get
            {
                using (var stream = new MemoryStream(_viewContent))
                {
                    return stream.Length;
                }
            }
        }

        public static async Task<byte[]> GetRemoteContentAsync(string url)
        {
            var byteResult = await RemoteFetch.FetchAsync(url, new WebRequestInit() { Accept = "application/text" });
            return byteResult;
        }

        public async Task GetView()
        {
            var query = new RazorLocationQuery() {Location = _viewPath};

            var doc = await _cache.GetObjectFromZeroFormatter<RazorLocation>(_viewPath);
            if (doc == null)
            {
                doc = await _store.FetchAsync(query);
                if (doc != null)
                {
                    byte[] viewContent;
                    if (!string.IsNullOrEmpty(doc.ContentUrl))
                    {
                        viewContent = await GetRemoteContentAsync(doc.ContentUrl);
                    }
                    else
                    {
                        viewContent = Encoding.UTF8.GetBytes(doc.Content);
                    }
                   
                    doc.ByteContent = viewContent;
                    await _cache.SetObjectAsZeroFormatter(_viewPath, doc, 3);
                }

            }
            if (doc != null)
            {
                _viewContent = doc.ByteContent;
                //                _viewContent = Encoding.UTF8.GetBytes(doc.Content);
                _lastModified = doc.LastModified;
                doc.LastRequested = DateTime.UtcNow;
                await _store.UpdateAsync(doc);
                _exists = true;
            }
        }


        public string Name => Path.GetFileName(_viewPath);

        public string PhysicalPath => null;
        public DateTimeOffset LastModified => _lastModified;
        public bool IsDirectory => false;
    }
}
