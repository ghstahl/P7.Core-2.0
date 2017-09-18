using P7.IdentityServer4.BiggyStore;

namespace Test.P7.IdentityServer4.BiggyStore
{
    class MyBiggyConfiguration : IIdentityServer4BiggyConfiguration
    {
        public string DatabaseName { get; set; }
        public string FolderStorage { get; set; }
        public string TenantId { get; set; }
    }
}