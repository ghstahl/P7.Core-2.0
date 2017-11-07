using GraphQL.Types;

namespace P7.Subscription
{
    public class SubscriptionQueryInput : InputObjectGraphType
    {
        public SubscriptionQueryInput()
        {
            Name = "subscriptionQueryInput";
            Field<NonNullGraphType<StringGraphType>>("id");
            Field<NonNullGraphType<MetaDataInput>>("metaData");
        }
    }

}