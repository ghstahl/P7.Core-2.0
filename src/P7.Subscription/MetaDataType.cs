using GraphQL.Types;
using P7.SimpleDocument.Store;

namespace P7.Subscription
{
    public class MetaDataType : ObjectGraphType<MetaData>
    {
        public MetaDataType()
        {
            Name = "metaData";
            Field(x => x.Category).Description("The Category of the subscription.");
            Field(x => x.Version).Description("The Version of the subscription.");
        }
    }
}