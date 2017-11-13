namespace P7.IdentityServer4.Common.Services
{
    public interface IPrivateClaimsScopesValidation
    {
        bool ValidatePrivateArbitraryScopes(string clientId, string[] arbitraryScopes);
        bool ValidatePrivateArbitraryClaims(string clientId, string[] arbitraryClaims);
    }
}