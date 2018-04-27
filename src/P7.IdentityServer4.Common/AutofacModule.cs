using Autofac;
using P7.IdentityServer4.Common.Extensions;
using P7.IdentityServer4.Common.Services;
using P7.IdentityServer4.Common.Stores;
using P7.IdentityServer4.Common.Validators;

namespace P7.IdentityServer4.Common
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.AddResourceService<DefaultResourcesStore>();
            builder.AddAdminResourceService<AdminResourceStore>();
          //  builder.AddCorsPolicyService<DefaultCorsPolicyService>();
            builder.AddResourceOwnerPasswordValidator<ArbitraryResourceOwnerPasswordValidator>();
            builder.AddClaimsService<CustomClaimsServiceHub>();

            builder.RegisterType<CustomArbitraryClaimsService>()
                .As<ICustomClaimsService>()
                .As<ICustomArbitraryClaimsService>();

            builder.RegisterType<CustomOpenIdClaimsService>()
                .As<ICustomClaimsService>()
                .As<ICustomOpenIdClaimsService>();

            builder.RegisterType<MyClientSecretValidator>().As<IRawClientSecretValidator>();
            builder.RegisterType<CustomArbitraryClaimsRequestValidator>();
        }
    }
}
