using System;
using System.Threading.Tasks;

namespace P7.SimpleRedirect.Core
{
    public interface ISimpleRedirectorStore
    {
        /// <summary>
        /// Finds the redirector record by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>null or RedirectRecord</returns>
        Task<SimpleRedirectRecord> FetchRedirectRecord(string key);
    }
}
