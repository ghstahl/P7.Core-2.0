using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace P7.IdentityServer4.Common
{
    public abstract class AbstractClientModel<TClaims, TSecrets, TStrings, TStringDictionary> : IClientModel
         where TClaims : class
         where TSecrets : class
         where TStrings : class
         where TStringDictionary : class
    {
        protected AbstractClientModel()
        {
        }

        protected AbstractClientModel(global::IdentityServer4.Models.Client client)
        {

            if (client != null)
            {
                AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime;
                AccessTokenLifetime = client.AccessTokenLifetime;
                AccessTokenType = client.AccessTokenType;
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser;
                AllowedCorsOrigins = Serialize(client.AllowedCorsOrigins);
                AllowedGrantTypes = Serialize(client.AllowedGrantTypes);
                AllowedScopes = Serialize(client.AllowedScopes);
                AllowOfflineAccess = client.AllowOfflineAccess;
                AllowPlainTextPkce = client.AllowPlainTextPkce;
                AllowRememberConsent = client.AllowRememberConsent;
                AlwaysSendClientClaims = client.AlwaysSendClientClaims;
                AuthorizationCodeLifetime = client.AuthorizationCodeLifetime;
                BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired;
                BackChannelLogoutUri = client.BackChannelLogoutUri;
                ConsentLifetime = client.ConsentLifetime;
                Claims = Serialize(client.Claims);
                ClientId = client.ClientId;
                ClientName = client.ClientName;
                ClientSecrets = Serialize(client.ClientSecrets);
                ClientUri = client.ClientUri;
                Enabled = client.Enabled;
                EnableLocalLogin = client.EnableLocalLogin;
                FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired;
                FrontChannelLogoutUri = client.FrontChannelLogoutUri;
                IdentityProviderRestrictions = Serialize(client.IdentityProviderRestrictions);
                IdentityTokenLifetime = client.IdentityTokenLifetime;
                IncludeJwtId = client.IncludeJwtId;
                LogoUri = client.LogoUri;
                PostLogoutRedirectUris = Serialize(client.PostLogoutRedirectUris);
                PrefixClientClaims = client.PrefixClientClaims;
                Properties = Serialize(client.Properties);
                ProtocolType = client.ProtocolType;
                RedirectUris = Serialize(client.RedirectUris);
                RefreshTokenExpiration = client.RefreshTokenExpiration;
                RefreshTokenUsage = client.RefreshTokenUsage;
                RequireClientSecret = client.RequireClientSecret;
                RequireConsent = client.RequireConsent;
                RequirePkce = client.RequirePkce;
                SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime;
                UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh;
            }
        }

        private TSecrets Serialize(ICollection<Secret> clientSecrets)
        {
            return Serialize(clientSecrets.ToList());
        }
        private TStringDictionary Serialize(IDictionary<string, string> dict)
        {
            return Serialize(dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }
        public abstract TStringDictionary Serialize(Dictionary<string, string> dict);
        private TStrings Serialize(ICollection<string> coll)
        {
            return Serialize(coll.ToList());
        }
        public abstract TStrings Serialize(List<string> stringList);
        public abstract Task<List<string>> DeserializeStringsAsync(TStrings obj);

        public abstract Task<IDictionary<string, string>> DeserializeStringDictionaryAsync(
            TStringDictionary obj);

        public abstract TClaims Serialize(List<Claim> claims);
        public TClaims Serialize(ICollection<Claim> claims)
        {
            return Serialize(claims.ToList());
        }
        public abstract Task<List<Claim>> DeserializeClaimsAsync(TClaims obj);
        public abstract TSecrets Serialize(List<Secret> secrets);
        public abstract Task<List<Secret>> DeserializeSecretsAsync(TSecrets obj);


        public async Task<global::IdentityServer4.Models.Client> MakeClientAsync()
        {
            var allowedCorsOrigins = await DeserializeStringsAsync(AllowedCorsOrigins);
            var allowedGrantTypes = await DeserializeStringsAsync(AllowedGrantTypes);
            var allowedScopes = await DeserializeStringsAsync(AllowedScopes);
            var claims = await DeserializeClaimsAsync(Claims);
            var clientSecrets = await DeserializeSecretsAsync(ClientSecrets);
            var identityProviderRestrictions = await DeserializeStringsAsync(IdentityProviderRestrictions);
            var postLogoutRedirectUris = await DeserializeStringsAsync(PostLogoutRedirectUris);
            var redirectUris = await DeserializeStringsAsync(RedirectUris);
            var properties = await DeserializeStringDictionaryAsync(Properties);

            Client client = new Client()
            {
                AbsoluteRefreshTokenLifetime = AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = AccessTokenLifetime,
                AccessTokenType = AccessTokenType,
                AllowAccessTokensViaBrowser = AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = allowedCorsOrigins,
                AllowedGrantTypes = allowedGrantTypes,
                AllowedScopes = allowedScopes,
                AllowOfflineAccess = AllowOfflineAccess,
                AllowPlainTextPkce = AllowPlainTextPkce,
                AllowRememberConsent = AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = AlwaysSendClientClaims,
                AuthorizationCodeLifetime = AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = BackChannelLogoutUri,
                ConsentLifetime = ConsentLifetime,
                Claims = claims,
                ClientId = ClientId,
                ClientName = ClientName,
                ClientSecrets = clientSecrets,
                ClientUri = ClientUri,
                Enabled = Enabled,
                EnableLocalLogin = EnableLocalLogin,
                FrontChannelLogoutSessionRequired = FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = FrontChannelLogoutUri,
                IdentityProviderRestrictions = identityProviderRestrictions,
                IdentityTokenLifetime = IdentityTokenLifetime,
                IncludeJwtId = IncludeJwtId,
                LogoUri = LogoUri,
                PostLogoutRedirectUris = postLogoutRedirectUris,
                PrefixClientClaims = PrefixClientClaims,
                Properties = properties,
                ProtocolType = ProtocolType,
                RedirectUris = redirectUris,
                RefreshTokenExpiration = RefreshTokenExpiration,
                RefreshTokenUsage = RefreshTokenUsage,
                RequireClientSecret = RequireClientSecret,
                RequireConsent = RequireConsent,
                RequirePkce = RequirePkce,
                SlidingRefreshTokenLifetime = SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = UpdateAccessTokenClaimsOnRefresh,
            };
            return client;
        }


        public int AbsoluteRefreshTokenLifetime { get; set; }
        public int AccessTokenLifetime { get; set; }
        public AccessTokenType AccessTokenType { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
        public TStrings AllowedCorsOrigins { get; set; }
        public TStrings AllowedGrantTypes { get; set; }
        public TStrings AllowedScopes { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public bool AllowPlainTextPkce { get; set; }
        public bool AllowRememberConsent { get; set; }
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public bool AlwaysSendClientClaims { get; set; }
        public int AuthorizationCodeLifetime { get; set; }
        public bool BackChannelLogoutSessionRequired { get; set; }
        public string BackChannelLogoutUri { get; set; }
        public int? ConsentLifetime { get; set; }
        public TClaims Claims { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public TSecrets ClientSecrets { get; set; }
        public string ClientUri { get; set; }
        public bool Enabled { get; set; }
        public bool EnableLocalLogin { get; set; }
        public string FrontChannelLogoutUri { get; set; }
        public bool FrontChannelLogoutSessionRequired { get; set; }
        public TStrings IdentityProviderRestrictions { get; set; }
        public int IdentityTokenLifetime { get; set; }
        public bool IncludeJwtId { get; set; }
        public string LogoUri { get; set; }
        public bool LogoutSessionRequired { get; set; }
        public string LogoutUri { get; set; }
        public TStrings PostLogoutRedirectUris { get; set; }
        public bool PrefixClientClaims { get; set; }
        public TStringDictionary Properties { get; set; }
        public string ProtocolType { get; set; }
        public TStrings RedirectUris { get; set; }
        public TokenExpiration RefreshTokenExpiration { get; set; }
        public TokenUsage RefreshTokenUsage { get; set; }
        public bool RequireClientSecret { get; set; }
        public bool RequireConsent { get; set; }
        public bool RequirePkce { get; set; }
        public int SlidingRefreshTokenLifetime { get; set; }
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

    }
}
