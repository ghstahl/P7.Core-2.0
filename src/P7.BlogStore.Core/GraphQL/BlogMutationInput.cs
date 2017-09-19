using GraphQL.Types;

namespace P7.BlogStore.Core.GraphQL
{
    public class BlogMutationInput : InputObjectGraphType
    {
        public BlogMutationInput()
        {
            Name = "blogMutationInput";
            Field<NonNullGraphType<StringGraphType>>("id");
            Field<NonNullGraphType<MetaDataInput>>("metaData");
            Field<NonNullGraphType<BlogInput>>("document");
        }
    }
    public class BlogInput : InputObjectGraphType
    {
        public BlogInput()
        {
            Name = "blogInput";
            Field<NonNullGraphType<StringGraphType>>("data");
            Field<NonNullGraphType<DateGraphType>>("timeStamp");
            Field<NonNullGraphType<StringGraphType>>("title");
            Field<NonNullGraphType<StringGraphType>>("summary");
            Field<ListGraphType<StringGraphType>>("tags");
            Field<ListGraphType<StringGraphType>>("categories");
        }
    }
}