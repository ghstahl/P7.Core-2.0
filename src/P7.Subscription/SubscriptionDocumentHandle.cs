using P7.SimpleDocument.Store;

namespace P7.Subscription
{
    public class SubscriptionDocumentHandle
    {
        public string Id { get; set; }
        public MetaData MetaData { get; set; }
        public object Value { get; set; }
    }
}