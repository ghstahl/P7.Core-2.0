using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P7.IdentityServer4.Common
{
    public interface IResourcesModel
    {
        Task<global::IdentityServer4.Models.Resources> MakeResourcesAsync();
    }
}
