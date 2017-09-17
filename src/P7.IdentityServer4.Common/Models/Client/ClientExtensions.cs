using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P7.IdentityServer4.Common
{
    public static class ClientExtensions
    {
        public static global::IdentityServer4.Models.Client ToClient(this ClientModel model)
        {
            var result = model.ToClientAsync();
            return result.Result;
        }
        public static async Task<global::IdentityServer4.Models.Client> ToClientAsync(this ClientModel model)
        {
            var result = await model.MakeClientAsync();
            return result;
        }
        public static ClientModel ToClientModel(this global::IdentityServer4.Models.Client model)
        {
            return new ClientModel(model);
        }
    }
}
