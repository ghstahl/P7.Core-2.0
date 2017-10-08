using System;
using System.Collections.Generic;
using System.Security.Claims;
using Castle.Components.DictionaryAdapter;
using IdentityServer4.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P7.Core.Utils;
using P7.IdentityServer4.Common;
using Shouldly;

namespace Test.P7.IdentityServer4.BiggyStore
{
    [TestClass]
    public class ModelTests
    {
        public static string UniqueGuid
        {
            get { return Guid.NewGuid().ToString(); }
        }

        public static ScopeModel NewScopeModel
        {
            get
            {

                var scope = new Scope()
                {
                    Description = UniqueGuid,
                    DisplayName = UniqueGuid,
                    Emphasize = true,
                    Name = UniqueGuid,
                    Required = true,
                    ShowInDiscoveryDocument = true,
                    UserClaims = new List<string>() { UniqueGuid, UniqueGuid }
                };

                var scopeModel = new ScopeModel(scope);
                return scopeModel;
            }
        }

        public static Secret NewSecret
        {
            get
            {
                var model = new Secret()
                {
                    Description = UniqueGuid,
                    Expiration = DateTime.UtcNow,
                    Type = UniqueGuid,
                    Value = UniqueGuid
                };
                return model;
            }
        }

        public static ApiResourceModel NewApiResourceModel
        {
            get
            {
                var model = new ApiResourceModel()
                {
                    DisplayName = UniqueGuid,
                    ApiSecrets = new EditableList<Secret>()
                    {
                        NewSecret,
                        NewSecret
                    },
                    Description = UniqueGuid,
                    Enabled = true,
                    Name = UniqueGuid,
                    Scopes = new List<ScopeModel>()
                    {
                        NewScopeModel,
                        NewScopeModel
                    },
                    UserClaims = new List<string>() { UniqueGuid, UniqueGuid }
                };
                return model;
            }
        }
        public static List<ApiResourceModel> MakeNewApiResourceModels(int count)
        {
            var final = new List<ApiResourceModel>();

            for (int i = 0; i < count; ++i)
            {
                final.Add(NewApiResourceModel);
            }
            return final;
        }

        public static IdentityResourceModel NewIdentityResourceModel
        {
            get
            {
                var model = new IdentityResourceModel()
                {
                    DisplayName = UniqueGuid,
                    Description = UniqueGuid,
                    Enabled = true,
                    Name = UniqueGuid,
                    UserClaims = new List<string>() { UniqueGuid, UniqueGuid },
                    Emphasize = true,
                    Required = true,
                    ShowInDiscoveryDocument = true
                };
                return model;
            }
        }
        public static List<IdentityResourceModel> MakeNewIdentityResourceModels(int count)
        {
            var final = new List<IdentityResourceModel>();

            for (int i = 0; i < count; ++i)
            {
                final.Add(NewIdentityResourceModel);
            }
            return final;
        }
        public static PersistedGrantModel NewPersistedGrantModel
        {
            get
            {
                return new PersistedGrantModel()
                {
                    ClientId = Guid.NewGuid().ToString(),
                    CreationTime = DateTime.UtcNow,
                    Data = Guid.NewGuid().ToString(),
                    Expiration = DateTime.UtcNow,
                    Key = Guid.NewGuid().ToString(),
                    SubjectId = Guid.NewGuid().ToString(),
                    Type = Guid.NewGuid().ToString()
                };
            }
        }

        public static List<PersistedGrantModel> MakeNewPersistedGrantModels(int count)
        {
            var final = new List<PersistedGrantModel>();

            for (int i = 0; i < count; ++i)
            {
                final.Add(NewPersistedGrantModel);
            }
            return final;
        }

        public static ResourcesModel NewResourcesModel
        {
            get
            {
                var model = new ResourcesModel()
                {
                    OfflineAccess = true,
                    IdentityResources = MakeNewIdentityResourceModels(10),
                    ApiResources = MakeNewApiResourceModels(10)
                };
                return model;
            }
        }

        public static Client NewClient
        {
            get
            {
                var client = new Client()
                {
                    AbsoluteRefreshTokenLifetime = 1,
                    AccessTokenLifetime = 1,
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowAccessTokensViaBrowser = true,
                    AllowedCorsOrigins = new List<string>() { UniqueGuid },
                    AllowedGrantTypes = new List<string>() { UniqueGuid },
                    AllowedScopes = new List<string>() { UniqueGuid },
                    AllowOfflineAccess = true,
                    AllowPlainTextPkce = true,
                    AllowRememberConsent = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AlwaysSendClientClaims = true,
                    AuthorizationCodeLifetime = 1,
                    BackChannelLogoutSessionRequired = true,
                    BackChannelLogoutUri = UniqueGuid,
                    ConsentLifetime = 1,
                    FrontChannelLogoutSessionRequired = true,
                    FrontChannelLogoutUri = UniqueGuid,
                    ClientId = UniqueGuid,
                    RequireClientSecret = true,
                    RequirePkce = true,
                    ProtocolType = UniqueGuid,
                    Properties = new Dictionary<string, string>() { {UniqueGuid,UniqueGuid}},
                    Claims = new List<Claim>() { new Claim(UniqueGuid, UniqueGuid) },
                    ClientName = UniqueGuid,
                    ClientSecrets = new List<Secret>() { new Secret(UniqueGuid, UniqueGuid) },
                    ClientUri = UniqueGuid,
                    EnableLocalLogin = true,
                    Enabled = true,
                    IdentityProviderRestrictions = new List<string>() { UniqueGuid },
                    IdentityTokenLifetime = 1,
                    IncludeJwtId = true,
                    LogoUri = UniqueGuid,
                    PostLogoutRedirectUris = new List<string>() { UniqueGuid },
                    RedirectUris = new List<string>() { UniqueGuid },
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RequireConsent = true,
                    SlidingRefreshTokenLifetime = 1,
                    UpdateAccessTokenClaimsOnRefresh = true
                };
                return client;
            }
        }



        [TestMethod]
        public void scope_model()
        {

            var scopeModel = NewScopeModel;
            var scopeModel2 = new ScopeModel(scopeModel.ToScope());
            var scopeModel3 = NewScopeModel;

            scopeModel2.ShouldBe(scopeModel);
            scopeModel3.ShouldNotBe(scopeModel);
        }
        [TestMethod]
        public void claim_model()
        {
            var claimModel = new ClaimModel()
            {
                Type = UniqueGuid,
                Value = UniqueGuid,
                ValueType = UniqueGuid
            };
            var claimModel2 = new ClaimModel(claimModel);
            var claimModel3 = new ClaimModel()
            {
                Type = UniqueGuid,
                Value = UniqueGuid,
                ValueType = UniqueGuid
            };



            claimModel2.ShouldBe(claimModel);
            claimModel2.ShouldNotBe(claimModel3);

        }
        [TestMethod]
        public void api_resource_model()
        {
            var model = NewApiResourceModel;
            var model2 = new ApiResourceModel(model.ToApiResource());

            var model3 = NewApiResourceModel;

            model.ShouldBe(model2);
            model.ShouldNotBe(model3);
        }
        [TestMethod]
        public void identity_resource_model()
        {
            var model = NewIdentityResourceModel;
            var model2 = new IdentityResourceModel(model.ToIdentityResource());

            var model3 = NewIdentityResourceModel;

            model.ShouldBe(model2);
            model.ShouldNotBe(model3);
        }
        [TestMethod]
        public void persisted_grant_model()
        {
            var model = NewPersistedGrantModel;
            var model2 = new PersistedGrantModel(model.ToPersistedGrant());

            var model3 = NewPersistedGrantModel;

            model.ShouldBe(model2);
            model.ShouldNotBe(model3);
        }
        [TestMethod]
        public void resources_model()
        {
            var model = NewResourcesModel;
            var model2 = new ResourcesModel(model.ToResources());

            var model3 = NewResourcesModel;

            model.ShouldBe(model2);
            model.ShouldNotBe(model3);
        }
        [TestMethod]
        public void client_model()
        {
            var model = ModelTests.NewClient.ToClientModel();
            var model2 = new ClientModel(model.ToClient());

            var model3 = ModelTests.NewClient.ToClientModel(); ;

            model.ShouldBe(model2);
            model.ShouldNotBe(model3);
        }

        [TestMethod]
        public void safe_equal()
        {
            string a = null;
            string b = "Hi";

            Assert.IsFalse(a.SafeEquals(b));

            Assert.IsFalse(b.SafeEquals(a));
            b = null;
            Assert.IsTrue(a.SafeEquals(b));
            a = "hi";
            b = "hi";
            Assert.IsTrue(a.SafeEquals(b));
            b = UniqueGuid;
            Assert.IsFalse(b.SafeEquals(a));
        }
    }
}
