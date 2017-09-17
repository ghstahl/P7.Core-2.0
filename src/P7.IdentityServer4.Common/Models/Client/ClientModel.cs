using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using P7.Core.Utils;

namespace P7.IdentityServer4.Common
{
    public class ClientModel :
        AbstractClientModel<
            List<ClaimModel>,
            List<Secret>,
            List<string>
        >
    {
        public ClientModel()
            : base()
        {
        }

        public ClientModel(Client client) : base(client)
        {
        }

        public override List<string> Serialize(List<string> stringList)
        {
            return stringList;
        }

        public override async Task<List<Claim>> DeserializeClaimsAsync(List<ClaimModel> obj)
        {
            return await Task.FromResult(obj == null ? null : obj.ToClaims());
        }

        public override List<ClaimModel> Serialize(List<Claim> claims)
        {
            return claims == null ? null : claims.ToClaimTypeRecords();
        }

        public override List<Secret> Serialize(List<Secret> secrets)
        {
            return secrets;
        }

        public override async Task<List<string>> DeserializeStringsAsync(List<string> obj)
        {
            return await Task.FromResult(obj);
        }

        public override async Task<List<Secret>> DeserializeSecretsAsync(List<Secret> obj)
        {
            return await Task.FromResult(obj);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ClientModel;
            if (other == null)
            {
                return false;
            }

            var result =
                AbsoluteRefreshTokenLifetime.SafeEquals(other.AbsoluteRefreshTokenLifetime)
                && AccessTokenLifetime.SafeEquals(other.AccessTokenLifetime)
                && AccessTokenType.SafeEquals(other.AccessTokenType)
                && AllowAccessTokensViaBrowser.SafeEquals(other.AllowAccessTokensViaBrowser)
                && AllowedCorsOrigins.SafeListEquals(other.AllowedCorsOrigins)
                && AllowedGrantTypes.SafeListEquals(other.AllowedGrantTypes)
                && AllowedScopes.SafeListEquals(other.AllowedScopes)
                && AllowOfflineAccess.SafeEquals(other.AllowOfflineAccess)
                && AllowPlainTextPkce.SafeEquals(other.AllowPlainTextPkce)
                && AllowRememberConsent.SafeEquals(other.AllowRememberConsent)
                && AlwaysSendClientClaims.SafeEquals(other.AlwaysSendClientClaims)
                && AuthorizationCodeLifetime.SafeEquals(other.AuthorizationCodeLifetime)
                && Claims.SafeListEquals(other.Claims)
                && ClientId.SafeEquals(other.ClientId)
                && ClientName.SafeEquals(other.ClientName)
                && ClientSecrets.SafeListEquals(other.ClientSecrets)
                && ClientUri.SafeEquals(other.ClientUri)
                && Enabled.SafeEquals(other.Enabled)
                && EnableLocalLogin.SafeEquals(other.EnableLocalLogin)
                && IdentityProviderRestrictions.SafeListEquals(other.IdentityProviderRestrictions)
                && IdentityTokenLifetime.SafeEquals(other.IdentityTokenLifetime)
                && IncludeJwtId.SafeEquals(other.IncludeJwtId)
                && LogoUri.SafeEquals(other.LogoUri)
                && LogoutSessionRequired.SafeEquals(other.LogoutSessionRequired)
                && LogoutUri.SafeEquals(other.LogoutUri)
                && PostLogoutRedirectUris.SafeListEquals(other.PostLogoutRedirectUris)
                && PrefixClientClaims.SafeEquals(other.PrefixClientClaims)
                && ProtocolType.SafeEquals(other.ProtocolType)
                && RedirectUris.SafeListEquals(other.RedirectUris)
                && RefreshTokenExpiration.SafeEquals(other.RefreshTokenExpiration)
                && RefreshTokenUsage.SafeEquals(other.RefreshTokenUsage)
                && RequireClientSecret.SafeEquals(other.RequireClientSecret)
                && RequireConsent.SafeEquals(other.RequireConsent)
                && RequirePkce.SafeEquals(other.RequirePkce)
                && SlidingRefreshTokenLifetime.SafeEquals(other.SlidingRefreshTokenLifetime)
                && UpdateAccessTokenClaimsOnRefresh.SafeEquals(other.UpdateAccessTokenClaimsOnRefresh);
            return result;
        }

        public override int GetHashCode()
        {
            var code = ClientId.GetHashCode();
            return code;
        }
    }
}