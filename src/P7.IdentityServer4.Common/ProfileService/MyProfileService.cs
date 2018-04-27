using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Newtonsoft.Json;
using P7.IdentityServer4.Common.Constants;

namespace P7.IdentityServer4.Common.ProfileService
{
    public class MyProfileService : IProfileService
    {
        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.Subject != null)
            {
                var query = from item in context.Subject.Claims
                    where item.Type == "amr"
                    select item.Value;
                var grantType = query.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(grantType) && 
                    string.Compare(grantType, AbritraryOwnerResourceConstants.GrantType,StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var queryClaims = from item in context.Subject.Claims
                        where item.Type == AbritraryOwnerResourceConstants.ArbitraryClaims
                                      select item.Value;
                    var claimsJson = queryClaims.FirstOrDefault();
                    if (claimsJson != null)
                    {
                        var values =
                            JsonConvert.DeserializeObject<Dictionary<string, string>>(claimsJson);
                        // paranoia check.  In no way can we allow creation which tries to spoof someone elses client_id.
                        var qq = from item in values
                            let c = item.Key
                            select c;

                        var queryF = from value in values
                            where String.Compare(value.Key, "client_id", StringComparison.OrdinalIgnoreCase) != 0
                            select value;
                        var trimmedClaims = queryF.ToList();
                        context.IssuedClaims.AddRange(trimmedClaims.Select(value => new Claim(value.Key, value.Value)));
                    }
                }
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {

        }
    }
}
