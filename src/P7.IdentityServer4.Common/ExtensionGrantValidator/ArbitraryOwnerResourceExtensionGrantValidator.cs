using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using P7.Core.Utils;
using P7.IdentityServer4.Common.Services;
using P7.IdentityServer4.Common.Validators;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using P7.IdentityServer4.Common.Constants;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace P7.IdentityServer4.Common.ExtensionGrantValidator
{
    public class ArbitraryOwnerResourceExtensionGrantValidator : IExtensionGrantValidator
    {
        
        private readonly ILogger<PublicRefreshTokenExtensionGrantValidator> _logger;
        private readonly IEventService _events;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly ITokenResponseGenerator _tokenResponseGenerator;
        private readonly IdentityServerOptions _options;
        private readonly IRawClientSecretValidator _clientSecretValidator;
        private ValidatedTokenRequest _validatedRequest;
        private readonly ICustomArbitraryClaimsService _customArbitraryClaimsService;
        private readonly CustomArbitraryClaimsRequestValidator _customArbitraryClaimsRequestValidator;
        private ISystemClock _clock;
 
        public ArbitraryOwnerResourceExtensionGrantValidator(
            IdentityServerOptions options,
            IClientStore clientStore,
            IResourceStore resourceStore,
            IEventService events,
            ISystemClock clock,
            IRawClientSecretValidator clientSecretValidator,
            ITokenResponseGenerator tokenResponseGenerator,
            ICustomArbitraryClaimsService customArbitraryClaimsService,
            CustomArbitraryClaimsRequestValidator customArbitraryClaimsRequestValidator,
            ILogger<PublicRefreshTokenExtensionGrantValidator> logger)
        {
            _logger = logger;
            _clock = clock;
            _events = events;
            _options = options;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _tokenResponseGenerator = tokenResponseGenerator;
            _clientSecretValidator = clientSecretValidator;
            _customArbitraryClaimsService = customArbitraryClaimsService;
            _customArbitraryClaimsRequestValidator = customArbitraryClaimsRequestValidator;
        }
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            _logger.LogDebug("Start token request validation");

            if (context == null) throw new ArgumentNullException(nameof(context));
            var raw = context.Request.Raw;
            _validatedRequest = new ValidatedTokenRequest
            {
                Raw = raw ?? throw new ArgumentNullException(nameof(raw)),
                Options = _options
            };

            var customTokenRequestValidationContext = new CustomTokenRequestValidationContext()
            {
                Result = new TokenRequestValidationResult(_validatedRequest)
            };

            await _customArbitraryClaimsRequestValidator.ValidateAsync(customTokenRequestValidationContext);
            if (customTokenRequestValidationContext.Result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, 
                    customTokenRequestValidationContext.Result.Error);
                return;
            }

            var clientValidationResult = await _clientSecretValidator.ValidateAsync(_validatedRequest.Raw);
            if (clientValidationResult == null) throw new ArgumentNullException(nameof(clientValidationResult));

            _validatedRequest.SetClient(clientValidationResult.Client, clientValidationResult.Secret);

            

            /////////////////////////////////////////////
            // check client protocol type
            /////////////////////////////////////////////
            if (_validatedRequest.Client.ProtocolType != IdentityServerConstants.ProtocolTypes.OpenIdConnect)
            {
                LogError("Client {clientId} has invalid protocol type for token endpoint: expected {expectedProtocolType} but found {protocolType}",
                    _validatedRequest.Client.ClientId,
                    IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    _validatedRequest.Client.ProtocolType);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidClient);
                return;
            }
            /////////////////////////////////////////////
            // check grant type
            /////////////////////////////////////////////
            var grantType = _validatedRequest.Raw.Get(OidcConstants.TokenRequest.GrantType);
            if (grantType.IsMissing())
            {
                LogError("Grant type is missing");
                context.Result = new GrantValidationResult(TokenRequestErrors.UnsupportedGrantType);
                return;
            }
            if(grantType.Length > _options.InputLengthRestrictions.GrantType)
            {
                LogError("Grant type is too long");
                context.Result = new GrantValidationResult(TokenRequestErrors.UnsupportedGrantType);
                return;
            }

            _validatedRequest.GrantType = grantType;

            var resource = await _resourceStore.GetAllResourcesAsync();
            // get user's identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, context.Request.Raw.Get("subject")),
                new Claim("sub", context.Request.Raw.Get("subject"))
            };
 
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            AugmentPrincipal(principal);
            //            _validatedRequest.Raw.Add();

            
          //  var userClaims = await _customArbitraryClaimsService.GetAccessTokenClaimsAsync(principal, resource, _validatedRequest);
            var userClaimsFinal = new List<Claim>()
            {
                new Claim(AbritraryOwnerResourceConstants.ArbitraryClaims, raw[AbritraryOwnerResourceConstants.ArbitraryClaims])
            };
            context.Result = new GrantValidationResult(context.Request.Raw.Get("subject"), AbritraryOwnerResourceConstants.GrantType, userClaimsFinal);
        }
        private void AugmentPrincipal(ClaimsPrincipal principal)
        {
            _logger.LogDebug("Augmenting SignInContext");
            AugmentMissingClaims(principal, _clock.UtcNow.UtcDateTime);
        }
        private void AugmentMissingClaims(ClaimsPrincipal principal, DateTime authTime)
        {
            var identity = principal.Identities.First();

            // ASP.NET Identity issues this claim type and uses the authentication middleware name
            // such as "Google" for the value. this code is trying to correct/convert that for
            // our scenario. IOW, we take their old AuthenticationMethod value of "Google"
            // and issue it as the idp claim. we then also issue a amr with "external"
            var amr = identity.FindFirst(ClaimTypes.AuthenticationMethod);
            if (amr != null &&
                identity.FindFirst(JwtClaimTypes.IdentityProvider) == null &&
                identity.FindFirst(JwtClaimTypes.AuthenticationMethod) == null)
            {
                _logger.LogDebug("Removing amr claim with value: {value}", amr.Value);
                identity.RemoveClaim(amr);

                _logger.LogDebug("Adding idp claim with value: {value}", amr.Value);
                identity.AddClaim(new Claim(JwtClaimTypes.IdentityProvider, amr.Value));

                _logger.LogDebug("Adding amr claim with value: {value}", P7.IdentityServer4.Common.Constants.Constants.ExternalAuthenticationMethod);
                identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, P7.IdentityServer4.Common.Constants.Constants.ExternalAuthenticationMethod));
            }

            if (identity.FindFirst(JwtClaimTypes.IdentityProvider) == null)
            {
                _logger.LogDebug("Adding idp claim with value: {value}", IdentityServerConstants.LocalIdentityProvider);
                identity.AddClaim(new Claim(JwtClaimTypes.IdentityProvider, IdentityServerConstants.LocalIdentityProvider));
            }

            if (identity.FindFirst(JwtClaimTypes.AuthenticationMethod) == null)
            {
                if (identity.FindFirst(JwtClaimTypes.IdentityProvider).Value == IdentityServerConstants.LocalIdentityProvider)
                {
                    _logger.LogDebug("Adding amr claim with value: {value}", OidcConstants.AuthenticationMethods.Password);
                    identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, OidcConstants.AuthenticationMethods.Password));
                }
                else
                {
                    _logger.LogDebug("Adding amr claim with value: {value}", P7.IdentityServer4.Common.Constants.Constants.ExternalAuthenticationMethod);
                    identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, P7.IdentityServer4.Common.Constants.Constants.ExternalAuthenticationMethod));
                }
            }

            if (identity.FindFirst(JwtClaimTypes.AuthenticationTime) == null)
            {
                var time = authTime.ToEpochTime().ToString();

                _logger.LogDebug("Adding auth_time claim with value: {value}", time);
                identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationTime, time, ClaimValueTypes.Integer));
            }
        }

        private TokenRequestValidationResult Invalid(string error, string errorDescription = null, Dictionary<string, object> customResponse = null)
        {
            return new TokenRequestValidationResult(_validatedRequest, error, errorDescription, customResponse);
        }
        private void LogError(string message = null, params object[] values)
        {
            if (message.IsPresent())
            {
                try
                {
                    _logger.LogError(message, values);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error logging {exception}", ex.Message);
                }
            }

            //  var details = new global::IdentityServer4.Logging.TokenRequestValidationLog(_validatedRequest);
            //  _logger.LogError("{details}", details);
        }
        public string GrantType => AbritraryOwnerResourceConstants.GrantType;
        internal class ResultDto
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string refresh_token { get; set; }
        }
    }
}