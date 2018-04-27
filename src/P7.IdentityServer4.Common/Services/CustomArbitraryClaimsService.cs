using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using P7.Core.Reflection;
using P7.Core.Utils;
using P7.Core.Logging;
using P7.IdentityServer4.Common.Constants;

namespace P7.IdentityServer4.Common.Services
{

    public class CustomArbitraryClaimsService : DefaultClaimsService, ICustomArbitraryClaimsService
    {
        public class LoggingEvents
        {
            public const int REQUIRED_ITEMS_MISSING = 1000;
        }

        public string Name => "arbitrary_claims_service";

        private readonly ILogger<CustomArbitraryClaimsService> _logger;
        private static List<string> _requiredArbitraryClaimsArguments;

        private static List<string> RequiredArbitraryClaimsArgument => _requiredArbitraryClaimsArguments ??
                                                                       (_requiredArbitraryClaimsArguments =
                                                                           new List<string>
                                                                           {
                                                                               AbritraryOwnerResourceConstants.ArbitraryClaims,
                                                                               "client_id"
                                                                           });

        private static List<string> _requiredArbitraryScopesArguments;

        private static List<string> RequiredArbitraryScopes => _requiredArbitraryScopesArguments ??
                                                               (_requiredArbitraryScopesArguments = new List<string>
                                                               {
                                                                   AbritraryOwnerResourceConstants.ArbitraryScopes,
                                                                   "client_id"
                                                               });

        private static List<string> _p7ClaimTypes;

        private static List<string> P7ClaimTypes
        {
            get
            {
                if (_p7ClaimTypes == null)
                {
                    var myConstants =
                        typeof(P7.IdentityServer4.Common.Constants.ClaimTypes).GetConstants<System.String>();
                    var values = myConstants.GetConstantsValues<System.String>();
                    _p7ClaimTypes = values.ToList();
                }

                return _p7ClaimTypes;
            }
        }

        public CustomArbitraryClaimsService(
            IProfileService profile,
            ILogger<CustomArbitraryClaimsService> logger) : base(profile, logger)
        {
            _logger = logger;
        }

        public async override Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject,
            Resources resources, ValidatedRequest request)
        {
            var arbitraryClaimsCheck = request.Raw.ContainsAll(RequiredArbitraryClaimsArgument);
            var arbitraryScopesCheck = request.Raw.ContainsAll(RequiredArbitraryScopes);
            if (!arbitraryClaimsCheck && !arbitraryScopesCheck)
            {
                var missing = string.Join(",", RequiredArbitraryClaimsArgument.ToArray());
                missing += ",";
                missing += string.Join(",", RequiredArbitraryScopes.ToArray());
                var ex = new Exception(string.Format("RequiredArgument failed need the following [{0}]", missing));
                _logger.LogError(LoggingEvents.REQUIRED_ITEMS_MISSING, ex);
                throw ex;
            }

            var result = base.GetAccessTokenClaimsAsync(subject, resources, request);
            var rr = request.Raw.AllKeys.ToDictionary(k => k, k => request.Raw[k]);
            List<Claim> finalClaims = new List<Claim>(result.Result);

            if (arbitraryScopesCheck)
            {
                var newScopes = rr[AbritraryOwnerResourceConstants.ArbitraryScopes].Split(new char[] {' ', '\t'},
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var scope in newScopes)
                {
                    finalClaims.Add(new Claim("scope", scope));
                }

            }

            Dictionary<string, string> values;
            if (arbitraryClaimsCheck)
            {
                values =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(rr[AbritraryOwnerResourceConstants.ArbitraryClaims]);
                // paranoia check.  In no way can we allow creation which tries to spoof someone elses client_id.
                var qq = from item in values
                    let c = item.Key
                    select c;

                var query = from value in values
                    where string.Compare(value.Key, "client_id", true) != 0
                    select value;
                var trimmedClaims = query.ToList();
                finalClaims.AddRange(trimmedClaims.Select(value => new Claim(value.Key, value.Value)));

            }

            finalClaims.Add(new Claim("arbitrary_namespace", rr["namespace"]));

            if (subject != null)
            {
                finalClaims.AddRange(subject.Claims.Where(p2 =>
                    finalClaims.All(p1 => p1.Type != p2.Type)));
            }

            // if we find any, than add them to the original and send that back.
            IEnumerable<Claim> claimresults = finalClaims;
            return claimresults;
        }
    }
}