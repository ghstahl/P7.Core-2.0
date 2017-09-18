using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using FakeItEasy;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P7.Core;
using P7.HugoStore.Core;
using P7.IdentityServer4.BiggyStore;
using P7.IdentityServer4.Common;
using P7.Store;
using Shouldly;

namespace Test.P7.IdentityServer4.BiggyStore
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class IdentityServer4Tests
    {
        public TenantDatabaseBiggyConfig GlobalTenantDatabaseBiggyConfig { get; set; }
        private string _targetFolder;
        protected string TargetFolder => _targetFolder;

        [TestInitialize]
        public void Initialize()
        {
            _targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source",
                DateTime.Now.ToString("yyyy-dd-M__HH-mm-ss") + "_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_targetFolder);
            GlobalTenantDatabaseBiggyConfig = new TenantDatabaseBiggyConfig();
            GlobalTenantDatabaseBiggyConfig.UsingFolder(TargetFolder);
            GlobalTenantDatabaseBiggyConfig.UsingTenantId(TenantDatabaseBiggyConfig.GlobalTenantId);
            IIdentityServer4BiggyConfiguration biggyConfiguration = new MyBiggyConfiguration()
            {
                FolderStorage = GlobalTenantDatabaseBiggyConfig.Folder,
                DatabaseName = GlobalTenantDatabaseBiggyConfig.Database,
                TenantId = GlobalTenantDatabaseBiggyConfig.TenantId.ToString()
            };

            var hostName = typeof(MyAutofacFactory).GetTypeInfo().Assembly.GetName().Name;
            var hostingEnvironment = A.Fake<IHostingEnvironment>();
            var httpContextAccessor = A.Fake<IHttpContextAccessor>();


            hostingEnvironment.ApplicationName = hostName;
            Global.HostingEnvironment = hostingEnvironment;
            AutofacStoreFactory = new MyAutofacFactory() { BiggyConfiguration = biggyConfiguration };

        }

        public MyAutofacFactory AutofacStoreFactory { get; set; }

        Client MakeNewClient3()
        {
            string clientId = Guid.NewGuid().ToString();
            var client = new Client()
            {
                AbsoluteRefreshTokenLifetime = 1,
                AccessTokenLifetime = 1,
                AccessTokenType = AccessTokenType.Jwt,
                AllowAccessTokensViaBrowser = true,
                AllowedCorsOrigins = new List<string>() { "a" },
                AllowedGrantTypes = new List<string>() { "a" },
                AllowedScopes = new List<string>() { "a" },
                AllowOfflineAccess = true,
                AllowPlainTextPkce = true,
                AllowRememberConsent = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AlwaysSendClientClaims = true,
                AuthorizationCodeLifetime = 1,
                BackChannelLogoutSessionRequired = true,
                BackChannelLogoutUri = "a",
                ClientId = clientId,
                FrontChannelLogoutUri = "a",
                FrontChannelLogoutSessionRequired = true,
                RequireClientSecret = true,
                RequirePkce = true,
                ProtocolType = "protocoltype",
                
                Claims = new List<Claim>() { new Claim("a-type", "a-value") },
                ClientName = "clientName",
                ClientSecrets = new List<Secret>() { new Secret("a-value", "a-description") },
                ClientUri = "clientUri",
                EnableLocalLogin = true,
                Enabled = true,
                IdentityProviderRestrictions = new List<string>() { "a" },
                IdentityTokenLifetime = 1,
                IncludeJwtId = true,
                LogoUri = "LogoUri",
               
                PostLogoutRedirectUris = new List<string>() { "a" },
                PrefixClientClaims = true,
                Properties = new Dictionary<string, string>() { { "a","a"} },
                RedirectUris = new List<string>() { "a" },
                RefreshTokenExpiration = TokenExpiration.Absolute,
                RefreshTokenUsage = TokenUsage.OneTimeOnly,
                RequireConsent = true,
                SlidingRefreshTokenLifetime = 1,
                UpdateAccessTokenClaimsOnRefresh = true
            };
            return client;
        }

        List<Client> MakeNewClients(int count)
        {
            var result = new List<Client>();
            for (int i = 0; i < count; ++i)
            {
                result.Add(ModelTests.NewClient);
            }
            return result;
        }
        [TestMethod]
        public async Task add_read_delete_client()
        {
            var fullClientStore = AutofacStoreFactory.Resolve<IFullClientStore>();
            var client = ModelTests.NewClient;
            await fullClientStore.InsertClientAsync(client);
            var result = await fullClientStore.FindClientByIdAsync(client.ClientId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ClientId == client.ClientId);

            await fullClientStore.DeleteClientByIdAsync(client.ClientId);

            result = await fullClientStore.FindClientByIdAsync(client.ClientId);
            Assert.IsNull(result);
        }
        [TestMethod]
        public async Task add_page_delete_clients()
        {
            var fullClientStore = AutofacStoreFactory.Resolve<IFullClientStore>();
            var clients = MakeNewClients(10);
            foreach (var client in clients)
            {
                await fullClientStore.InsertClientAsync(client);
                var result = await fullClientStore.FindClientByIdAsync(client.ClientId);
                Assert.IsNotNull(result);
                Assert.IsTrue(result.ClientId == client.ClientId);
            }

            var page = await fullClientStore.PageAsync(11, null);
            Assert.AreEqual<int>(clients.Count, page.Count);
            Assert.IsNull(page.CurrentPagingState);
            Assert.IsNull(page.PagingState);

            foreach (var client in clients)
            {
                await fullClientStore.DeleteClientByIdAsync(client.ClientId);
                var result = await fullClientStore.FindClientByIdAsync(client.ClientId);
                Assert.IsNull(result);
            }
        }


        [TestMethod]
        public async Task add_read_delete_persisted_grant()
        {
            var theStore = AutofacStoreFactory.Resolve<IPersistedGrantStore>();
            var grantModel = ModelTests.NewPersistedGrantModel;
            var grant = grantModel.ToPersistedGrant();

            await theStore.StoreAsync(grant);

            var result = await theStore.GetAsync(grant.Key);
            var resultModel = PersistedGrantExtensions.ToPersistedGrantModel((PersistedGrant) result);

            grantModel.ShouldBe(resultModel, (string)null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ClientId == grant.ClientId);
            Assert.IsTrue(result.SubjectId == grant.SubjectId);
            Assert.IsTrue(result.Type == grant.Type);

            await theStore.RemoveAsync(grant.Key);

            result = await theStore.GetAsync(grant.Key);
            Assert.IsNull(result);
        }
        [TestMethod]
        public async Task add_read_delete_persisted_grants_by_subjectid()
        {
            var theStore = AutofacStoreFactory.Resolve<IPersistedGrantStore>();
            var grants = ModelTests.MakeNewPersistedGrantModels(10).ToPersistedGrants();

            var clientId = Guid.NewGuid().ToString();
            var subjectId = Guid.NewGuid().ToString();
            var type = Guid.NewGuid().ToString();
            int i = 0;
            foreach (var grant in grants)
            {
                grant.ClientId = clientId;
                if (i % 2 == 0)
                {
                    grant.SubjectId = subjectId;
                    grant.Type = type;
                }
                await theStore.StoreAsync(grant);
                ++i;
            }

            var results = await theStore.GetAllAsync(subjectId);
            Assert.IsNotNull(results);
            Assert.AreEqual(grants.Count / 2, Enumerable.Count<PersistedGrant>(results));

            foreach (var result in results)
            {
                ShouldBeStringTestExtensions.ShouldBe(result.SubjectId, subjectId);
            }




            await theStore.RemoveAllAsync(subjectId, clientId);
            results = await theStore.GetAllAsync(subjectId);
            Assert.IsFalse(Enumerable.Any<PersistedGrant>(results));
        }

        [TestMethod]
        public async Task add_read_delete_persisted_grants_by_subjectid_and_type()
        {
            var theStore = AutofacStoreFactory.Resolve<IPersistedGrantStore>();
            var grants = ModelTests.MakeNewPersistedGrantModels(10).ToPersistedGrants();

            var clientId = Guid.NewGuid().ToString();
            var subjectId = Guid.NewGuid().ToString();
            var type = Guid.NewGuid().ToString();
            int i = 0;
            foreach (var grant in grants)
            {
                grant.SubjectId = subjectId;
                grant.ClientId = clientId;
                if (i % 2 == 0)
                {
                    grant.Type = type;
                }
                await theStore.StoreAsync(grant);
                ++i;
            }

            var results = await theStore.GetAllAsync(subjectId);

            Assert.IsNotNull(results);
            Assert.AreEqual(grants.Count, Enumerable.Count<PersistedGrant>(results));


            foreach (var result in results)
            {
                ShouldBeStringTestExtensions.ShouldBe(result.SubjectId, subjectId);
            }

            await theStore.RemoveAllAsync(subjectId, clientId, type);
            results = await theStore.GetAllAsync(subjectId);
            foreach (var result in results)
            {
                ShouldBeStringTestExtensions.ShouldBe(result.SubjectId, subjectId);
            }

            Assert.IsTrue(Enumerable.Any<PersistedGrant>(results));
            Assert.AreEqual(Enumerable.Count<PersistedGrant>(results), grants.Count / 2);
        }
        [TestMethod]
        public async Task add_read_delete_persisted_grants()
        {
            var theStore = AutofacStoreFactory.Resolve<IPersistedGrantStore>();
            var grants = ModelTests.MakeNewPersistedGrantModels(10).ToPersistedGrants();

            var clientId = Guid.NewGuid().ToString();
            var subjectId = Guid.NewGuid().ToString();
            var type = Guid.NewGuid().ToString();
            int i = 0;
            foreach (var grant in grants)
            {
                grant.SubjectId = subjectId;
                grant.ClientId = clientId;
                if (i % 2 == 0)
                {
                    grant.Type = type;
                }
                await theStore.StoreAsync(grant);
                ++i;
            }

            var expectedGrantModels = grants.ToPersistedGrantModels();


            var result = await theStore.GetAllAsync(subjectId);
            var resultList = Enumerable.ToList<PersistedGrant>(result);

            var readGrantModels = resultList.ToPersistedGrantModels();

            string additionalInfo = null;
            expectedGrantModels.ShouldBe(readGrantModels, additionalInfo);



            Assert.IsNotNull(result);
            Assert.AreEqual(grants.Count, resultList.Count());



            foreach (var grant in grants)
            {
                await theStore.RemoveAsync(grant.Key);
            }

            result = await theStore.GetAllAsync(subjectId);

            Assert.IsFalse(Enumerable.Any<PersistedGrant>(result));
        }
        private List<IdentityResource> MakeNewIdentityResources(int count)
        {
            var final = new List<IdentityResource>();

            for (int i = 0; i < count; ++i)
            {
                final.Add(MakeNewIdentityResource());
            }
            return final;
        }
        private IdentityResource MakeNewIdentityResource()
        {
            IdentityResource identityResource = new IdentityResource()
            {
                Description = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),
                Emphasize = true,
                Enabled = true,
                Name = Guid.NewGuid().ToString(),
                Required = true,
                ShowInDiscoveryDocument = true,
                UserClaims = new List<string>()
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()
                }
            };
            return identityResource;
        }

        [TestMethod]
        public async Task identity_resource_store_fullboat()
        {
            var resourceStore = AutofacStoreFactory.Resolve<IResourceStore>();
            var adminResourceStore = AutofacStoreFactory.Resolve<IAdminResourceStore>();

            var identityResources = MakeNewIdentityResources(10);
            var apiResources = MakeNewApiResources(10);
            int i = 0;
            foreach (var identityResource in identityResources)
            {
                identityResource.Name = "Test:" + i;
                ++i;
                await adminResourceStore.IdentityResourceStore.InsertIdentityResourceAsync(identityResource);
            }
            i = 0;
            foreach (var apiResource in apiResources)
            {
                apiResource.Name = "Test:" + i;
                ++i;
                apiResource.Scopes.Add(new Scope() { Name = "Test" });
                await adminResourceStore.ApiResourceStore.InsertApiResourceAsync(apiResource);
            }

            var resource = await resourceStore.GetAllResourcesAsync();
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ApiResources.Count, apiResources.Count);
            Assert.AreEqual(resource.IdentityResources.Count, identityResources.Count);

            var filteredApiResources = await resourceStore.FindApiResourcesByScopeAsync(new List<string>() { "Test" });
            Assert.AreEqual(resource.ApiResources.Count, Enumerable.Count<ApiResource>(filteredApiResources));

            ApiResource apiResource2 = await resourceStore.FindApiResourceAsync("Test:0");
            Assert.IsNotNull(apiResource2);
            Assert.AreEqual(apiResource2.Name, "Test:0");

            var identityResources2 = await resourceStore.FindIdentityResourcesByScopeAsync(new List<string>() { "Test:0" });
            Assert.IsNotNull(identityResources2);
            Assert.AreEqual(1, Enumerable.Count<IdentityResource>(identityResources2));
            Assert.AreEqual(Enumerable.FirstOrDefault<IdentityResource>(identityResources2).Name, "Test:0");
        }


        [TestMethod]
        public async Task identity_resource_store_test()
        {
            var resourceStore = AutofacStoreFactory.Resolve<IResourceStore>();
            var adminResourceStore = AutofacStoreFactory.Resolve<IAdminResourceStore>();
            Assert.IsNotNull(resourceStore);
            Assert.IsNotNull(adminResourceStore);

            IdentityResource identityResource = MakeNewIdentityResource();

            await adminResourceStore.IdentityResourceStore.InsertIdentityResourceAsync(identityResource);

            var dd = await adminResourceStore.IdentityResourceStore.PageAsync(10, null);
            Assert.AreEqual<int>(1, dd.Count);
            var item = Enumerable.FirstOrDefault<IdentityResource>(dd);
            Assert.AreEqual(item.Name, identityResource.Name);

            await adminResourceStore.IdentityResourceStore.DeleteIdentityResourceByNameAsync(item.Name);
            dd = await adminResourceStore.IdentityResourceStore.PageAsync(10, null);
            Assert.AreEqual<int>(0, dd.Count);
        }
        private List<ApiResource> MakeNewApiResources(int count)
        {
            var final = new List<ApiResource>();

            for (int i = 0; i < count; ++i)
            {
                final.Add(MakeNewApiResource());
            }
            return final;
        }

        private ApiResource MakeNewApiResource()
        {
            ApiResource apiResource = new ApiResource()
            {
                Description = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),

                Enabled = true,
                Name = Guid.NewGuid().ToString(),

                UserClaims = new List<string>()
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()
                },
                ApiSecrets = new List<Secret>()
                {
                    new Secret()
                    {
                        Description = Guid.NewGuid().ToString(),
                        Expiration = DateTime.UtcNow,
                        Type = Guid.NewGuid().ToString(),
                        Value = Guid.NewGuid().ToString()
                    }
                },
                Scopes = new List<Scope>()
                {
                    new Scope()
                    {
                        Description = Guid.NewGuid().ToString(),
                        DisplayName = Guid.NewGuid().ToString(),
                        Emphasize = true,
                        Name = Guid.NewGuid().ToString(),
                        Required = true,
                        ShowInDiscoveryDocument = true,
                        UserClaims = new List<string>() {Guid.NewGuid().ToString(), Guid.NewGuid().ToString()}
                    }
                }
            };
            return apiResource;
        }

        [TestMethod]
        public async Task api_resource_store_test()
        {
            var resourceStore = AutofacStoreFactory.Resolve<IResourceStore>();
            var adminResourceStore = AutofacStoreFactory.Resolve<IAdminResourceStore>();
            Assert.IsNotNull(resourceStore);
            Assert.IsNotNull(adminResourceStore);
            var apiResource = MakeNewApiResource();
            await adminResourceStore.ApiResourceStore.InsertApiResourceAsync(apiResource);

            var dd = await adminResourceStore.ApiResourceStore.PageAsync(10, null);
            Assert.AreEqual<int>(1, dd.Count);
            var item = Enumerable.FirstOrDefault<ApiResource>(dd);
            Assert.AreEqual<string>(item.Name, apiResource.Name);

            await adminResourceStore.ApiResourceStore.DeleteApiResourceByNameAsync(item.Name);
            dd = await adminResourceStore.ApiResourceStore.PageAsync(10, null);
            Assert.AreEqual<int>(0, dd.Count);
        }



        private Consent MakeNewConsent()
        {
            return new Consent()
            {
                ClientId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.UtcNow,
                Scopes = new List<string>() { "a-scope" },
                SubjectId = Guid.NewGuid().ToString()
            };
        }

        [TestMethod]
        public void paging_state_conversions()
        {
            PagingState pagingStateExpected = new PagingState() { CurrentIndex = 1234 };
            var bytes = pagingStateExpected.Serialize();
            var pagingState = bytes.DeserializePageState();

            pagingState.ShouldBe(pagingStateExpected);

            var psString = Convert.ToBase64String(bytes);
            bytes = Convert.FromBase64String(psString);
            pagingState = bytes.DeserializePageState();

            pagingState.ShouldBe(pagingStateExpected);

            var urlEncodedPagingState = WebUtility.UrlEncode(psString);
            var psStringUrlDecoded = WebUtility.UrlDecode(urlEncodedPagingState);

            psStringUrlDecoded.ShouldBe(psString);
            bytes = Convert.FromBase64String(psStringUrlDecoded);
            pagingState = bytes.DeserializePageState();
            pagingState.ShouldBe(pagingStateExpected);
        }
    }
}