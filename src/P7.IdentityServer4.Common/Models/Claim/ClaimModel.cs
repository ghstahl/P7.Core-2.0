using System.Security.Claims;
using IdentityServer4.Models;
using P7.Core.Utils;

namespace P7.IdentityServer4.Common
{
    public class ClaimModel
    {
        public ClaimModel()
        {
        }

        public ClaimModel(Claim claim)
        {
            Type = claim.Type;
            Value = claim.Value;
            ValueType = claim.ValueType;
        }
        public ClaimModel(ClaimModel claim)
        {
            Type = claim.Type;
            Value = claim.Value;
            ValueType = claim.ValueType;
        }

        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as ClaimModel;
            if (other == null)
            {
                return false;
            }

            var result = Type.SafeEquals(other.Type)
                         && Value.SafeEquals(other.Value)
                         && ValueType.SafeEquals(other.ValueType);
            return result;
        }

        public override int GetHashCode()
        {
            var code = Type.GetHashCode();
            return code;
        }
    }
}