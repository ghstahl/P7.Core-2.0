using P7.Core.Utils;


namespace P7.IdentityServer4.Common
{
    public class PersistedGrantModel : AbstractPersistedGrantModel
    {
        public PersistedGrantModel()
        {
        }
        public PersistedGrantModel(global::IdentityServer4.Models.PersistedGrant grant):base(grant)
        {
        }

        public override bool Equals(object obj)
        {
            var other = obj as PersistedGrantModel;
            if (other == null)
            {
                return false;
            }


            var result = ClientId.SafeEquals(other.ClientId)
                   && CreationTime.SafeEquals(other.CreationTime)
                   && Data.SafeEquals(other.Data)
                   && Expiration.SafeEquals(other.Expiration)
                   && Key.SafeEquals(other.Key)
                   && SubjectId.SafeEquals(other.SubjectId)
                   && Type.SafeEquals(other.Type);
            return result;
        }

        public override int GetHashCode()
        {
            var code = ClientId.GetHashCode() ^
                   CreationTime.GetHashCode() ^
                   Data.GetHashCode() ^
                   Expiration.GetHashCode() ^
                   Key.GetHashCode() ^
                   SubjectId.GetHashCode() ^
                   Type.GetHashCode();
            return code;
        }
    }
}

 