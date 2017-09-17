using GraphQL.Types;

namespace P7.Globalization
{
    public class ResourceQueryInput : InputObjectGraphType
    {
        public ResourceQueryInput()
        {
            Name = "ResourceQueryInput";
            Field<NonNullGraphType<StringGraphType>>("id");
            Field<NonNullGraphType<StringGraphType>>("treatment");
            Field<StringGraphType>("culture");
        }
    }
}