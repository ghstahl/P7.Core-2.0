using GraphQL.Types;

namespace P7.Identity
{
    public class IdentityQueryInput : InputObjectGraphType
    {
        public IdentityQueryInput()
        {
            Name = "identityQueryInput";
            Field<NonNullGraphType<StringGraphType>>("id");
        }
    }
}
