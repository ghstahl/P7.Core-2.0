using System;
using System.Collections.Generic;
using P7.IdentityServer4.AspNetIdentity.Configuration;

namespace P7.IdentityServer4.AspNetIdentity.Stores
{
    public interface IIdentityServerDiscoveryStore
    {
        OpenidConfiguration GetOpenidConfiguration();

    }
}
