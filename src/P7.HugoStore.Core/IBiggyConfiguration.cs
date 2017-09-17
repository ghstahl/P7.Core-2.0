namespace P7.HugoStore.Core
{
    public interface IBiggyConfiguration
    {
        string DatabaseName { get;  }
        string FolderStorage { get; }
        string TenantId { get; }
    }
}