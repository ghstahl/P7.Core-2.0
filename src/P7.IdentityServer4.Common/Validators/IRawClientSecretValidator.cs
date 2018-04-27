using System.Collections.Specialized;
using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace P7.IdentityServer4.Common.Validators
{
    /// <summary>
    /// Validator for handling client authentication
    /// </summary>
    public interface IRawClientSecretValidator
    {
        /// <summary>
        /// Tries to authenticate a client based on the incoming request
        /// </summary>
        /// <param name="nvc">The namevalue collection from the request.</param>
        /// <returns></returns>
        Task<ClientSecretValidationResult> ValidateAsync(NameValueCollection nvc);
    }
}