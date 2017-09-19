using GraphQL.Types;

namespace P7.BlogStore.Core.GraphQL
{
    public class BlogQueryInput : InputObjectGraphType
    {
        public BlogQueryInput()
        {
            Name = "blogQueryInput";
            Field<NonNullGraphType<StringGraphType>>("id");
        }
    }
}
