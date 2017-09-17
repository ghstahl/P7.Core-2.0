using Autofac;
using IdentityServer4.Stores;
using P7.IdentityServer4.BiggyStore.Extensions;
using P7.IdentityServer4.Common;

namespace P7.IdentityServer4.BiggyStore
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.AddIdentityServer4BiggyClientStores();
            builder.AddIdentityServer4BiggyPersistedGrantStore();
            builder.AddIdentityServer4BiggyResourceStores();
        }
    }
}