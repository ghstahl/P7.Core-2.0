using System;
using Newtonsoft.Json;

namespace P7.Store
{
    public class DocumentBase : IDocumentBase
    {
        [JsonIgnore]
        public Guid Id_G
        {
            get
            {
                if (string.IsNullOrEmpty(Id))
                    return Guid.Empty;

                return Guid.Parse(Id);
            }
        }

        public virtual string Id { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as DocumentBase;
            if (other == null)
            {
                return false;
            }
            if (Id != other.Id)
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class DocumentBaseWithTenant : DocumentBase,IDocumentBaseWithTenant
    {
        [JsonIgnore]
        public Guid TenantId_G
        {
            get
            {
                if (string.IsNullOrEmpty(TenantId))
                    return Guid.Empty;

                return Guid.Parse(TenantId);
            }
        }
        public virtual string TenantId { get; set; }
        public override bool Equals(object obj)
        {
            var other = obj as DocumentBaseWithTenant;
            if (other == null)
            {
                return false;
            }
            if (TenantId != other.TenantId)
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            if (TenantId == null)
            {
                hash ^= "".GetHashCode();
            }
            else
            {
                hash ^= TenantId.GetHashCode();

            }
            return hash;
        }
    }
}