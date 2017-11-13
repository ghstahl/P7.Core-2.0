namespace P7.IdentityServer4.Common.Services
{
    public interface IPrivateScopeValidation
    {
        bool ValidateArbitraryScopes(string clientId, string[] arbitraryScopes);
    }
}