using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using P7.IdentityServer4.Common.Services;

namespace P7.IdentityServer4.Common.Validators
{
    public class CustomArbitraryClaimsRequestValidator
    {
        private readonly ILogger<CustomArbitraryClaimsService> _logger;

        private static List<string> _requiredArbitraryArguments;
        private static List<string> RequiredArbitraryArguments => _requiredArbitraryArguments ??
                                                                  (_requiredArbitraryArguments =
                                                                      new List<string>
                                                                      {
                                                                          "subject",
                                                                          "client_id",
                                                                          "client_secret",
                                                                          "arbitrary_claims"
                                                                      });
        public CustomArbitraryClaimsRequestValidator(
            ILogger<CustomArbitraryClaimsService> logger)
        {
            _logger = logger;
        }
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            var raw = context.Result.ValidatedRequest.Raw;
            var rr = raw.AllKeys.ToDictionary(k => k, k => raw[(string) k]);
            var error = false;
            var los = new List<string>();

            var result = RequiredArbitraryArguments.Except(rr.Keys);
            if (result.Any())
            {
                error = true;
                los.AddRange(result.Select(item => $"{item} is missing!"));

            }
            if (error)
            {
                context.Result.IsError = true;
                context.Result.Error = String.Join<string>(" | ", los); ;
            }
            return Task.CompletedTask;
        }
    }
}