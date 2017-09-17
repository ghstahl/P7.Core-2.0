using Autofac;
using IdentityServer4.Stores;
using P7.HugoStore.Core;
using P7.IdentityServer4.BiggyStore;
using P7.IdentityServer4.Common;
using P7.IdentityServer4.Common.Stores;

namespace P7.IdentityServer4.BiggyStore.Extensions
{
    public static class IdentityServer4Extensions
    {
        /// <summary>
        /// Adds the in identityserver 4 client stores.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static ContainerBuilder AddIdentityServer4BiggyClientStores(this ContainerBuilder builder)
        {
            builder.RegisterType<ClientStore>().As<IFullClientStore>();
            builder.RegisterType<ClientStore>().As<IClientStore>();
            return builder;
        }
        /// <summary>
        /// Adds the in Persisted Grant Store.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static ContainerBuilder AddIdentityServer4BiggyPersistedGrantStore(this ContainerBuilder builder)
        {
            builder.RegisterType<PersistedGrantStore>().As<IPersistedGrantStore>();
            return builder;
        }
        /// <summary>
        /// Adds the in Persisted Grant Store.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static ContainerBuilder AddIdentityServer4BiggyResourceStores(this ContainerBuilder builder)
        {
            builder.RegisterType<IdentityResourceStore>().As<IIdentityResourceStore>();
            builder.RegisterType<ApiResourceStore>().As<IApiResourceStore>();
            return builder;
        }

        /// <summary>
        /// Adds IdentityServer4 biggy configuration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="dbPath">The full path to where biggy store the data.</param>
        /// <returns></returns>
        public static ContainerBuilder AddIdentityServer4BiggyConfiguration(this ContainerBuilder builder, string dbPath)
        {
            var globalTenantDatabaseBiggyConfig = new TenantDatabaseBiggyConfig();
            globalTenantDatabaseBiggyConfig.UsingFolder(dbPath);
            globalTenantDatabaseBiggyConfig.UsingTenantId(TenantDatabaseBiggyConfig.GlobalTenantId);
            IIdentityServer4BiggyConfiguration biggyConfiguration = new MyBiggyConfiguration()
            {
                FolderStorage = globalTenantDatabaseBiggyConfig.Folder,
                DatabaseName = globalTenantDatabaseBiggyConfig.Database
            };

            builder.Register(c => biggyConfiguration)
                .As<IIdentityServer4BiggyConfiguration>()
                .SingleInstance();
            return builder;
        }
    }
}


