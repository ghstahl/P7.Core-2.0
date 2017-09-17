using System.Collections.Generic;
using System.Linq;

namespace P7.IdentityServer4.Common
{
    public static class ClaimExtensions
    {
        public static System.Security.Claims.Claim ToClaim(this ClaimModel model)
        {
            return new System.Security.Claims.Claim(model.Type, model.Value, model.ValueType);
        }
        public static List<System.Security.Claims.Claim> ToClaims(this List<ClaimModel> models)
        {
            var query = from item in models
                select ToClaim(item);
            return query.ToList();
        }
        public static ClaimModel ToClaimTypeRecord(this System.Security.Claims.Claim claim)
        {
            return new ClaimModel(claim);
        }
        public static List<ClaimModel> ToClaimTypeRecords(this List<System.Security.Claims.Claim> claims)
        {
            var query = from item in claims
                select item.ToClaimTypeRecord();
            return query.ToList();
        }
    }
}