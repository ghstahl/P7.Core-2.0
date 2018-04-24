using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using P7.IdentityServer4.Common.Services;

namespace P7.IdentityServer4.Common.Validators
{
    public class MyCustomTokenRequestValidator : ICustomTokenRequestValidator
    {
        private IClientNamespaceValidation _clientNamespaceValidation;
        public MyCustomTokenRequestValidator(IClientNamespaceValidation clientNamespaceValidation)
        {
            
            _clientNamespaceValidation = clientNamespaceValidation;
        }
        /// <summary>
        /// Custom validation logic for a token request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The validation result
        /// </returns>
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            var raw = context.Result.ValidatedRequest.Raw;
            var rr = raw.AllKeys.ToDictionary(k => k, k => raw[k]);

            if (rr.ContainsKey("handler"))
            {
                var namespaces = rr["namespace"].Split(new char[] { ' ', '\t' },
                    StringSplitOptions.RemoveEmptyEntries);
                var clientId = rr["client_id"];

                if (!_clientNamespaceValidation.ValidateClientNamespace(rr["client_id"], namespaces))
                {
                    context.Result.IsError = true;
                    context.Result.Error = "namespace not allowed for this client";
                }

            }
           
            return Task.CompletedTask;
        }
    }
}
