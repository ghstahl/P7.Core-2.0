using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using P7.RazorProvider.Store.Core.Interfaces;
using P7.RazorProvider.Store.Core.Models;


namespace P7.Razor.FileProvider
{
    public class RazorFileInfo : IFileInfo
    {
        private IRazorLocationStore _store;
        private string _viewPath;
        private byte[] _viewContent;
        private DateTimeOffset _lastModified;
        private bool _exists;
        public RazorFileInfo(IRazorLocationStore store, string viewPath)
        {
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

        public async Task GetView()
        {
            var query = new RazorLocationQuery() { Location = _viewPath };

            var doc = await _store.FetchAsync(query);
            if (doc != null)
            {
                _viewContent = Encoding.UTF8.GetBytes(doc.Content);
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
