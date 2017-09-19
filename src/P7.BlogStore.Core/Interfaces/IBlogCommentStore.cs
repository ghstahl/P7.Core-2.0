using System;
using System.Threading.Tasks;
using P7.Store;


namespace P7.BlogStore.Core
{
    public interface IBlogCommentStore
    {
        #region BlogComment

        /// <summary>
        /// Creates a new BlogComment
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="blogComment"></param>
        /// <returns></returns>
        Task InsertAsync(Guid blogId, BlogComment blogComment);

        /// <summary>
        /// Updates a BlogComment
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="blogComment"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid blogId, BlogComment blogComment);


        /// <summary>
        /// Delete the BlogComment
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid blogId, Guid id);

        /// <summary>
        /// Finds the BlogComment
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BlogComment> FetchAsync(Guid blogId, Guid id);

        /// <summary>
        /// Pages all documents
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pagingState"></param>
        /// <returns></returns>
        Task<IPage<BlogComment>> PageAsync(
            Guid blogId,
            int pageSize,
            byte[] pagingState);

        #endregion
    }
}