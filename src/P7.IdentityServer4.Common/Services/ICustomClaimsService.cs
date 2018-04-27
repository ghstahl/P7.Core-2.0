using IdentityServer4.Services;

namespace P7.IdentityServer4.Common.Services
{
    public interface ICustomOpenIdClaimsService : ICustomClaimsService
    {
        string Name { get; }
    }
    public interface ICustomArbitraryClaimsService : ICustomClaimsService
    {
        string Name { get; }
    }
    public interface ICustomClaimsService : IClaimsService
    {
        string Name { get; }
    }
}