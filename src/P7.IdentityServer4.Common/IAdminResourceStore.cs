namespace P7.IdentityServer4.Common
{
    public interface IAdminResourceStore
    {
        IIdentityResourceStore IdentityResourceStore { get; }
        IApiResourceStore ApiResourceStore { get; }
    }
}