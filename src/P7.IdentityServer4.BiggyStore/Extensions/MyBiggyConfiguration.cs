namespace P7.IdentityServer4.BiggyStore.Extensions
{
    internal class MyBiggyConfiguration : IIdentityServer4BiggyConfiguration
    {
        public string DatabaseName { get; set; }
        public string FolderStorage { get; set; }
        public string TenantId { get; set; }
    }
}