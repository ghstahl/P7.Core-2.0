using System;
using System.Collections.Generic;

namespace P7.BlogStore.Core
{
    public class BlogsPageHandle
    {
        public int PageSize { get; set; }
        public string PagingState { get; set; }
        public DateTime TimeStampLowerBoundary { get; set; }
        public DateTime TimeStampUpperBoundary { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        public BlogsPageHandle()
        {
        }

        public BlogsPageHandle(BlogsPageHandle doc)
        {
            this.Categories = doc.Categories;
            this.Tags = doc.Tags;
            this.PageSize = doc.PageSize;
            this.PagingState = doc.PagingState;
            this.TimeStampLowerBoundary = doc.TimeStampLowerBoundary;
            this.TimeStampUpperBoundary = doc.TimeStampUpperBoundary;
        }
    }
    public class BlogsPageByNumberHandle
    {
        public int PageSize { get; set; }
        public int Page { get; set; }
        public DateTime TimeStampLowerBoundary { get; set; }
        public DateTime TimeStampUpperBoundary { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        public BlogsPageByNumberHandle()
        {
        }

        public BlogsPageByNumberHandle(BlogsPageByNumberHandle doc)
        {
            this.Categories = doc.Categories;
            this.Tags = doc.Tags;
            this.PageSize = doc.PageSize;
            this.Page = doc.Page;
            this.TimeStampLowerBoundary = doc.TimeStampLowerBoundary;
            this.TimeStampUpperBoundary = doc.TimeStampUpperBoundary;
        }
    }
}