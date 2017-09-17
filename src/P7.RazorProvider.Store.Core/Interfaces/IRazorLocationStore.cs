using System.Threading.Tasks;
using P7.RazorProvider.Store.Core.Models;
using P7.Store;

namespace P7.RazorProvider.Store.Core.Interfaces
{
    public interface IRemoteRazorLocationStore : IRazorLocationStore
    {
        Task LoadRemoteDataAsync(string url);
    }
    public interface IRazorLocationStore 
    {
        /// <summary>
        /// Creates a new record
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task InsertAsync(RazorLocation document);

        /// <summary>
        /// Updates a new record
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task UpdateAsync(RazorLocation document);


        /// <summary>
        /// Delete the SimpleDocument
        /// </summary>
        /// <param name="query"></param>        
        /// <returns></returns>
        Task DeleteAsync(RazorLocationQuery query );

        /// <summary>
        /// Finds the SimpleDocument
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<RazorLocation> FetchAsync(RazorLocationQuery query);

        /// <summary>
        /// Pages all SimpleDocuments
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pagingState"></param>
        /// <returns></returns>
        Task<IPage<RazorLocation>> PageAsync(int pageSize,
            byte[] pagingState);

       
    }
}
