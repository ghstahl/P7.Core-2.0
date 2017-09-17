using System;
using P7.Core.Utils;
using P7.Store;

namespace P7.SimpleDocument.Store
{
    public interface ISimpleDocumentBase<T> : IDocumentBaseWithTenant
    {
        MetaData MetaData { get; }
        T Document { get; }

    }

    public class SimpleDocument<T> : DocumentBaseWithTenant, ISimpleDocumentBase<T> where T:IComparable
    {
        public SimpleDocument() { }

        public SimpleDocument(MetaData metaData,T document)
        {
            Document = document;
            MetaData = metaData;
        }

        public MetaData MetaData { get; set; }   
        public T Document { get; set; }
        public override bool Equals(object obj)
        {
            var other = obj as SimpleDocument<T>;
            if (other == null)
            {
                return false;
            }
            if (!MetaData.SafeEquals(other.MetaData))
            {
                return false;
            }
            if (!Document.SafeEquals(other.Document))
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash ^= MetaData.GetHashCode();
            hash ^= Document.GetHashCode();
            return hash;
        }
    }
}