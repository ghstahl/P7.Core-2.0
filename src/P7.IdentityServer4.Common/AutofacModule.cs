using Autofac;
using P7.IdentityServer4.Common.Extensions;
using P7.IdentityServer4.Common.Services;
using P7.IdentityServer4.Common.Stores;

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
            builder.AddCustomClaimsService<CustomArbitraryClaimsService>();
            builder.AddCustomClaimsService<CustomOpenIdClaimsService>();

        }
    }
}
