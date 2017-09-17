using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Validation;
using P7.Core.Utils;

namespace P7.IdentityServer4.Common.ExtensionGrantValidator
{
    internal class TokenRequestValidationLog
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string GrantType { get; set; }
        public string Scopes { get; set; }

        public string AuthorizationCode { get; set; }
        public string RefreshToken { get; set; }

        public string UserName { get; set; }
        public IEnumerable<string> AuthenticationContextReferenceClasses { get; set; }
        public string Tenant { get; set; }
        public string IdP { get; set; }

        public Dictionary<string, string> Raw { get; set; }

        static readonly string[] SensitiveValuesFilter = {
            OidcConstants.TokenRequest.Password,
            OidcConstants.TokenRequest.ClientAssertion
        };

        public TokenRequestValidationLog(ValidatedTokenRequest request)
        {
            Raw = request.Raw.ToScrubbedDictionary(SensitiveValuesFilter);

            if (request.Client != null)
            {
                ClientId = request.Client.ClientId;
                ClientName = request.Client.ClientName;
            }

            if (request.Scopes != null)
            {
                Scopes = request.Scopes.ToSpaceSeparatedString();
            }

            GrantType = request.GrantType;
            AuthorizationCode = request.AuthorizationCodeHandle;
            RefreshToken = request.RefreshTokenHandle;
            UserName = request.UserName;
        }

        public override string ToString()
        {
            return LogSerializer.Serialize(this);
        }
    }
}