using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P7.Store;

namespace P7.SimpleDocument.Store
{
    public interface ISimpleDocumentStore<T> where T:IComparable
    {
        /// <summary>
        /// Creates a new SimpleDocument
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task InsertAsync(SimpleDocument<T> document);

        /// <summary>
        /// Updates a new SimpleDocument
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task UpdateAsync(SimpleDocument<T> document);


        /// <summary>
        /// Delete the SimpleDocument
        /// </summary>
        /// <param name="id"></param>        
        /// <returns></returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Finds the SimpleDocument
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SimpleDocument<T>> FetchAsync(Guid id);

        /// <summary>
        /// Pages all SimpleDocuments
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pagingState"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        Task<IPage<SimpleDocument<T>>> PageAsync(int pageSize,
            byte[] pagingState,
            MetaData metaData = null);

        /// <summary>
        /// Pages all SimpleDocuments
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pagingState"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        Task<ICollection<SimpleDocument<T>>> PageAsync(int pageSize,
            int page,
            MetaData metaData = null);
    }
    public interface ISimpleDocumentStoreWithTenant<T> where T : IComparable
    {
        /// <summary>
        /// Gets this stores tenantId
        /// </summary>
        /// <returns></returns>
        Task<string> GetTenantIdAsync();

        /// <summary>
        /// Creates a new SimpleDocument
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task InsertAsync(SimpleDocument<T> document);

        /// <summary>
        /// Updates a new SimpleDocument
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task UpdateAsync(SimpleDocument<T> document);


        /// <summary>
        /// Delete the SimpleDocument
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="id"></param>        
        /// <returns></returns>
        Task DeleteAsync(Guid tenantId, Guid id);

        /// <summary>
        /// Finds the SimpleDocument
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SimpleDocument<T>> FetchAsync(Guid tenantId, Guid id);

        /// <summary>
        /// Pages all SimpleDocuments
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pagingState"></param>
        /// <param name="tenantId"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        Task<IPage<SimpleDocument<T>>> PageAsync(int pageSize,
            byte[] pagingState,
            Guid? tenantId = null,
            MetaData metaData = null);

        /// <summary>
        /// Pages all SimpleDocuments
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        /// <param name="tenantId"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        Task<ICollection<SimpleDocument<T>>> PageAsync(int pageSize,
            int page,
            Guid? tenantId = null,
            MetaData metaData = null);
    }
}
