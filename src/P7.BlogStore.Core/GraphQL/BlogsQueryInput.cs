using GraphQL.Types;

namespace P7.BlogStore.Core.GraphQL
{
    public class BlogsQueryInput : InputObjectGraphType
    {
        public BlogsQueryInput()
        {
            Name = "blogsQueryInput";
            Field<NonNullGraphType<IntGraphType>>("pageSize");
            Field<StringGraphType>("pagingState");
            Field<DateGraphType>("timestampLowerBoundary");
            Field<DateGraphType>("timestampUpperBoundary");
            Field<ListGraphType<StringGraphType>>("categories");
            Field<ListGraphType<StringGraphType>>("tags");

        }
    }
    public class BlogsPageQueryInput : InputObjectGraphType
    {
        public BlogsPageQueryInput()
        {
            Name = "blogsPageQueryInput";
            Field<NonNullGraphType<IntGraphType>>("pageSize");
            Field<NonNullGraphType<IntGraphType>>("page");
            Field<DateGraphType>("timestampLowerBoundary");
            Field<DateGraphType>("timestampUpperBoundary");
            Field<ListGraphType<StringGraphType>>("categories");
            Field<ListGraphType<StringGraphType>>("tags");

        }
    }
}