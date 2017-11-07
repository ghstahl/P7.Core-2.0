using System;
using P7.SimpleDocument.Store;
using P7.Core.Utils;

namespace P7.Subscription
{
    public class SubscriptionQueryHandle: IComparable
    {
        public SubscriptionQueryHandle() { }
        public string Id { get; set; }
        public SubscriptionQueryHandle(string id, MetaData metaData)
        {
            Id = id;
            MetaData = metaData;
        }

        public MetaData MetaData { get; set; }
      
        public override bool Equals(object obj)
        {
            var other = obj as SubscriptionQueryHandle;
            if (other == null)
            {
                return false;
            }
            if (!MetaData.SafeEquals(other.MetaData))
            {
                return false;
            }

            return true;
        }
        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash ^= MetaData.GetHashCode();
            hash ^= Id.GetHashCode();

            return hash;
        }

        public int CompareTo(object obj)
        {
            if (Equals(obj))
                return 0;
            return -1;
        }
    }
}