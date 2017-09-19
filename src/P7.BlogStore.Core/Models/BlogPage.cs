using P7.SimpleDocument.Store;
using System.Collections.Generic;

namespace P7.BlogStore.Core.Models
{
    public class BlogPage
    {
        public string CurrentPagingState { get; set; }
        public string PagingState { get; set; }
        public List<SimpleDocument<Blog>> Blogs { get; set; }
    }
}