using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using Autofac;
using FakeItEasy;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using P7.GraphQLCore;
using Microsoft.Extensions.DependencyModel;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using P7.BlogStore.Hugo;
using P7.Core;
using P7.GraphQLCore.Stores;
using P7.GraphQLCore.Validators;
using P7.HugoStore.Core;
using Test.P7.GraphQLCoreTest.GraphQLAuth;

namespace Test.P7.GraphQLCoreTest
{
    public class MyAutofacFactory
    {
        public IBlogStoreBiggyConfiguration BlogStoreBiggyConfiguration { get; set; }

        private IContainer _autofacContainer;

        public IContainer AutofacContainer
        {
            get
            {
                if (_autofacContainer == null)
                {
                    var builder = new ContainerBuilder();
                    List<Assembly> assemblies = new List<Assembly>
                    {
                        Assembly.Load(new AssemblyName("P7.Core")),
                        Assembly.Load(new AssemblyName("P7.Globalization")),
                        Assembly.Load(new AssemblyName("P7.BlogStore.Hugo")),
                        Assembly.Load(new AssemblyName("P7.GraphQLCore")),
                        Assembly.Load(new AssemblyName("P7.BlogStore.Core"))
                    };
                    builder.RegisterAssemblyModules(assemblies.ToArray());

                    builder.RegisterInstance(BlogStoreBiggyConfiguration).As<IBlogStoreBiggyConfiguration>();

                    builder.RegisterInstance(Global.HostingEnvironment).As<IHostingEnvironment>();
                    var httpContextAccessor = A.Fake<IHttpContextAccessor>();
                    var httpContext = A.Fake<HttpContext>();
                    A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);
                    var featureCollection = A.Fake<IFeatureCollection>();
                    var requestCultureFeature = A.Fake<IRequestCultureFeature>();
                    var requestCulture = new RequestCulture(new CultureInfo("en-US"));

                    A.CallTo(() => httpContext.Features).Returns(featureCollection);
                    A.CallTo(() => featureCollection.Get<IRequestCultureFeature>()).Returns(requestCultureFeature);
                    A.CallTo(() => requestCultureFeature.RequestCulture).Returns(requestCulture);

                    var user = A.Fake<ClaimsPrincipal>();
                    A.CallTo(() => httpContext.User).Returns(user);

                    var claims = A.Fake<List<Claim>>();
                    var identity = A.Fake<IIdentity>();
                    A.CallTo(() => identity.IsAuthenticated).Returns(true);
                    A.CallTo(() => identity.AuthenticationType).Returns("google");
                    A.CallTo(() => identity.Name).Returns("gName");
                    A.CallTo(() => user.Identity).Returns(identity);

                    var claim = new Claim(ClaimTypes.NameIdentifier, "herb");
                    claims.Add(claim);
                    A.CallTo(() => user.Claims).Returns(claims);

                    builder.RegisterInstance(httpContextAccessor).As<IHttpContextAccessor>();
                    builder.RegisterType<GraphQLUserContext>();

                    var locOptions = new LocalizationOptions();
                    var options = A.Fake<IOptions<LocalizationOptions>>();
                    A.CallTo(() => options.Value).Returns(locOptions);

                    builder.RegisterInstance(options).As<IOptions<LocalizationOptions>>();
                    builder.RegisterType<ResourceManagerStringLocalizerFactory>()
                        .As<IStringLocalizerFactory>()
                        .SingleInstance();
                    builder.RegisterType<MemoryCacheOptions>()
                        .As<IOptions<MemoryCacheOptions>>();
                    builder.RegisterType<MemoryCache>()
                        .As<IMemoryCache>();

                    builder.RegisterType<TestGraphQLAuthStore>()
                        .As<IAllUsersOptOutGraphQLAuthStore>()
                        .SingleInstance();

                    builder.RegisterType<TestAllUsersOptOutGraphQLClaimsPrincipalAuthStore>()
                        .As<IAllUsersOptOutGraphQLClaimsPrincipalAuthStore>()
                        .SingleInstance();


                    builder.RegisterType<InMemoryGraphQLFieldAuthority>()
                        .As<IGraphQLFieldAuthority>()
                        .SingleInstance();
                    var loggerFactory = A.Fake<ILoggerFactory>();
                    builder.RegisterInstance(loggerFactory).As<ILoggerFactory>();
                    var container = builder.Build();

                    _autofacContainer = container;
                }

                return _autofacContainer;
            }
        }

        public T Resolve<T>()
        {
            AutofacContainer.Resolve<T>();
            return AutofacContainer.Resolve<T>();
        }
        public IEnumerable<T> ResolveMany<T>()
        {
            return Resolve<IEnumerable<T>>();
        }
    }
}