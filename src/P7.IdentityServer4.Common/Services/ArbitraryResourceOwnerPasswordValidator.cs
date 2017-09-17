using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;

namespace P7.IdentityServer4.Common.Services
{
    public class ArbitraryResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            context.Result = new GrantValidationResult(context.UserName, OidcConstants.AuthenticationMethods.Password);
            return Task.FromResult(0);
        }
    }
}
