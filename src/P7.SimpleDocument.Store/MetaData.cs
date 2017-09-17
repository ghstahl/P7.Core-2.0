using P7.Core.Utils;

namespace P7.SimpleDocument.Store
{
    public class MetaData
    {
        public string Category { get; set; }
        public string Version { get; set; }
        public override bool Equals(object obj)
        {
            var other = obj as MetaData;
            if (other == null)
            {
                return false;
            }
            if (!Category.IsEqual(other.Category))
            {
                return false;
            }
            if (!Version.IsEqual(other.Version))
            {
                return false;
            }
            return true;
        }

         
        public override int GetHashCode()
        {
            return Category.GetHashCode() ^ Version.GetHashCode();
        }
    }
}