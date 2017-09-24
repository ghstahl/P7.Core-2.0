using System;
using ZeroFormatter;


namespace P7.Store
{
    [ZeroFormattable]
    public class PagingState
    {
        [Index(0)]
        public virtual int CurrentIndex { get; set; }

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