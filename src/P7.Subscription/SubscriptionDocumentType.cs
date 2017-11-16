using GraphQL.Types;
using P7.GraphQLCore.Types;

namespace P7.Subscription
{
    public class SubscriptionDocumentType : ObjectGraphType<SubscriptionDocumentHandle>
    {
        public SubscriptionDocumentType()
        {
            Name = "subscriptionDocument";
           
            Field(x => x.Id).Description("The Id of the Subscription.");
            Field<MetaDataType>("metaData", "The MetaData of the Subscription.");
            Field<DynamicType>("value","The value");
        }
    }
}