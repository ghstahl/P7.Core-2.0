using IdentityServer4.Services;

namespace P7.IdentityServer4.Common.Services
{
    public interface ICustomClaimsService : IClaimsService
    {
        string Name { get; }
    }
}