using System;
using ProtoBuf;

namespace P7.Store
{
    [ProtoContract]
    public class PagingState
    {
        [ProtoMember(1)]
        public int CurrentIndex { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as PagingState;
            if (other == null)
            {
                return false;
            }
            if (CurrentIndex != other.CurrentIndex)
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            return CurrentIndex.GetHashCode();
        }
    }
}