using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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

namespace P7.IdentityServer4.Common.ExtensionGrantValidator
{
    public class PublicRefreshTokenExtensionGrantValidator : IExtensionGrantValidator
    {
        internal class ResultDto
        { 
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string refresh_token { get; set; }
        }

        private readonly ILogger<PublicRefreshTokenExtensionGrantValidator> _logger;
        private readonly IEventService _events;
        private readonly IdentityServerOptions _options;
        private readonly IRefreshTokenStore _refreshTokenStore;
        private readonly IProfileService _profile;
        private readonly IClientStore _clientStore;
        private readonly ITokenResponseGenerator _tokenResponseGenerator;
        private readonly ITokenValidator _tokenValidator;
        private ValidatedTokenRequest _validatedRequest;
        private const string PrependPublic = "public-";
        private const int PrependPublicIndex = 7;
        public PublicRefreshTokenExtensionGrantValidator(
            IdentityServerOptions options,
            IRefreshTokenStore refreshTokenStore,
            ITokenResponseGenerator tokenResponseGenerator,
            ITokenValidator tokenValidator,
            IProfileService profile,
            IClientStore clientStore,
            IEventService events,
            ILogger<PublicRefreshTokenExtensionGrantValidator> logger)
        {
            _logger = logger;
            _events = events;
            _options = options;
            _refreshTokenStore = refreshTokenStore;
            _tokenValidator = tokenValidator;
            _profile = profile;
            _clientStore = clientStore;
            _tokenResponseGenerator = tokenResponseGenerator;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var refreshToken = context.Request.Raw.Get("refresh_token");
            var clientId = context.Request.Raw.Get("client_id");

            if (string.IsNullOrEmpty(refreshToken))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }
            if (string.IsNullOrEmpty(clientId))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidClient);
                return;
            }
            var index = clientId.IndexOf(PrependPublic);
            if (index != 0)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidClient);
                return;
            }

            var client = await _clientStore.FindClientByIdAsync(clientId);
            if (client == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidClient);
                return;
            }

            var originalClientId = clientId.Substring(PrependPublicIndex);
            if (string.IsNullOrEmpty(originalClientId))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidClient);
                return;
            }
            var originalClient = await _clientStore.FindClientByIdAsync(originalClientId);
            if (originalClient == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidClient);
                return;
            }


            NameValueCollection nvc = new NameValueCollection
            {
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, refreshToken}
            };
            _validatedRequest = new ValidatedTokenRequest
            {
                Raw = nvc,
                Client = originalClient,
                Options = _options
            };
            var result = await ValidateRefreshTokenRequestAsync(nvc, originalClient);
            if (result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            result.ValidatedRequest.GrantType = OidcConstants.GrantTypes.RefreshToken;
            result.ValidatedRequest.AccessTokenLifetime = result.ValidatedRequest.Client.AccessTokenLifetime;
            var refreshResponse = await _tokenResponseGenerator.ProcessAsync(result);

            var dto = new ResultDto
            {

                access_token = refreshResponse.AccessToken,
                refresh_token = refreshResponse.RefreshToken,
                expires_in = refreshResponse.AccessTokenLifetime,
                token_type = OidcConstants.TokenResponse.BearerTokenType
            };
            var response = new Dictionary<string, object>
            {
                {"inner_response", dto}
            };

            context.Result = new GrantValidationResult(customResponse: response);
        }

        public string GrantType => "public_refresh_token";

        private async Task<TokenRequestValidationResult> ValidateRefreshTokenRequestAsync(NameValueCollection parameters, Client client = null)
        {
           
            _logger.LogDebug("Start validation of refresh token request");
            var refreshTokenHandle = parameters.Get(OidcConstants.TokenRequest.RefreshToken);
            var refreshTokenValidatorResult = await _tokenValidator.ValidateRefreshTokenAsync(refreshTokenHandle, client);

            if (refreshTokenValidatorResult.IsError)
            {
                return Invalid(refreshTokenValidatorResult.Error);
            }


            _validatedRequest.RefreshTokenHandle = refreshTokenHandle;
            var refreshToken = refreshTokenValidatorResult.RefreshToken;
           

            /////////////////////////////////////////////
            // check if client belongs to requested refresh token
            /////////////////////////////////////////////
            if (_validatedRequest.Client.ClientId != refreshToken.ClientId)
            {
                LogError("{0} tries to refresh token belonging to {1}", _validatedRequest.Client.ClientId, refreshToken.ClientId);
                await RaiseRefreshTokenRefreshFailureEventAsync(refreshTokenHandle, "Invalid client binding");

                return Invalid(OidcConstants.TokenErrors.InvalidGrant);
            }

            /////////////////////////////////////////////
            // check if client still has offline_access scope
            /////////////////////////////////////////////
            if (!_validatedRequest.Client.AllowOfflineAccess)
            {
                LogError("{clientId} does not have access to offline_access scope anymore", _validatedRequest.Client.ClientId);
                var error = "Client does not have access to offline_access scope anymore";
                await RaiseRefreshTokenRefreshFailureEventAsync(refreshTokenHandle, error);

                return Invalid(OidcConstants.TokenErrors.InvalidGrant);
            }

            _validatedRequest.RefreshToken = refreshToken;

            /////////////////////////////////////////////
            // make sure user is enabled
            /////////////////////////////////////////////
            var subject = _validatedRequest.RefreshToken.Subject;
            var isActiveCtx = new IsActiveContext(subject, _validatedRequest.Client, IdentityServerConstants.ProfileIsActiveCallers.RefreshTokenValidation);
            await _profile.IsActiveAsync(isActiveCtx);

            if (isActiveCtx.IsActive == false)
            {
                LogError("{subjectId} has been disabled", _validatedRequest.RefreshToken.SubjectId);
                var error = "User has been disabled: " + _validatedRequest.RefreshToken.SubjectId;
                await RaiseRefreshTokenRefreshFailureEventAsync(refreshTokenHandle, error);

                return Invalid(OidcConstants.TokenErrors.InvalidGrant);
            }

            _validatedRequest.Subject = subject;

            _logger.LogDebug("Validation of refresh token request success");
            return Valid();
        }
        private TokenRequestValidationResult Valid(Dictionary<string, object> customResponse = null)
        {
            return new TokenRequestValidationResult(_validatedRequest, customResponse);
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

            var details = new TokenRequestValidationLog(_validatedRequest);
            _logger.LogError("{details}", details);
        }
        private void LogSuccess()
        {
            var details = new TokenRequestValidationLog(_validatedRequest);
            _logger.LogInformation("Token request validation success\n{details}", details);
        }

        private async Task RaiseSuccessfulResourceOwnerAuthenticationEventAsync(string userName, string subjectId)
        {
            await _events.RaiseSuccessfulResourceOwnerPasswordAuthenticationEventAsync(userName, subjectId);
        }

        private async Task RaiseFailedResourceOwnerAuthenticationEventAsync(string userName, string error)
        {
            await _events.RaiseFailedResourceOwnerPasswordAuthenticationEventAsync(userName, error);
        }

        private async Task RaiseFailedAuthorizationCodeRedeemedEventAsync(string handle, string error)
        {
            await _events.RaiseFailedAuthorizationCodeRedeemedEventAsync(_validatedRequest.Client, handle, error);
        }

        private async Task RaiseSuccessfulAuthorizationCodeRedeemedEventAsync()
        {
            await _events.RaiseSuccessAuthorizationCodeRedeemedEventAsync(_validatedRequest.Client, _validatedRequest.AuthorizationCodeHandle);
        }

        private async Task RaiseRefreshTokenRefreshFailureEventAsync(string handle, string error)
        {
            await _events.RaiseFailedRefreshTokenRefreshEventAsync(_validatedRequest.Client, handle, error);
        }
    }
}

