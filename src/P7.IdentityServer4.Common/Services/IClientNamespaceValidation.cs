namespace P7.IdentityServer4.Common.Services
{
    public interface IClientNamespaceValidation
    {
        bool ValidateClientNamespace(string clientId, string[] namespaces);
    }
}