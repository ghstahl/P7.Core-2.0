using GraphQL.Types;

namespace P7.BlogStore.Core.GraphQL
{
    public class MetaDataInput : InputObjectGraphType
    {
        public MetaDataInput()
        {
            Name = "metaDataInput";
            Field<NonNullGraphType<StringGraphType>>("category");
            Field<NonNullGraphType<StringGraphType>>("version");
        }
    }
}