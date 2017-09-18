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

namespace P7.IdentityServer4.Common.Services
{
    public class CustomOpenIdClaimsService : DefaultClaimsService, ICustomClaimsService
    {
        public class LoggingEvents
        {
            public const int REQUIRED_ITEMS_MISSING = 1000;
        }

        public CustomOpenIdClaimsService(IProfileService profile, ILogger<CustomOpenIdClaimsService> logger) : base(profile, logger)
        {
            _logger = logger;
        }
        public string Name => "arbitrary-openid-claims";
        private readonly ILogger<CustomOpenIdClaimsService> _logger;
        private static List<string> _requiredArguments;

        private static List<string> RequiredArgument => _requiredArguments ?? (_requiredArguments = new List<string>
        {
            "openid-connect-token"
        });

        private static List<string> _p7ClaimTypes;
        private static List<string> P7ClaimTypes
        {
            get
            {
                if (_p7ClaimTypes == null)
                {
                    var myConstants = typeof(P7.IdentityServer4.Common.Constants.ClaimTypes).GetConstants<System.String>();
                    var values = myConstants.GetConstantsValues<System.String>();
                    _p7ClaimTypes = values.ToList();
                }
                return _p7ClaimTypes;
            }
        }

        public override Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, Resources resources, ValidatedRequest request)
        {
         
            if (!request.Raw.ContainsAny(RequiredArgument))
            {
                var ex = new Exception(string.Format("RequiredArgument failed need the following [{0}]", string.Join(",", RequiredArgument.ToArray())));
                _logger.LogError(LoggingEvents.REQUIRED_ITEMS_MISSING,ex);
                throw ex;
            }
            var result = base.GetAccessTokenClaimsAsync(subject, resources, request);
            var rr = request.Raw.AllKeys.ToDictionary(k => k, k => request.Raw[k]);
            List<Claim> finalClaims = new List<Claim>(result.Result);
            string output = JsonConvert.SerializeObject(rr);
            finalClaims.Add(new Claim(P7.IdentityServer4.Common.Constants.ClaimTypes.ClientRequestNameValueCollection, output));

            if (subject != null)
            {
                finalClaims.AddRange(subject.Claims.Where(p2 =>
                  finalClaims.All(p1 => p1.Type != p2.Type)));
            }
            // if we find any, than add them to the original and send that back.
            IEnumerable<Claim> claimresults = finalClaims;
            var taskResult = Task.FromResult(claimresults);
            return taskResult;
        }
    }
}