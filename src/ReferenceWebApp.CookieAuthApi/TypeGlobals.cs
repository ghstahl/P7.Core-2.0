using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Internal;

namespace ReferenceWebApp.CookieAuthApi
{
    public class TypeGlobals
    {
        public static IEnumerable<ApplicationPart> ApplicationParts
        {
            get
            {
                return DefaultAssemblyPartDiscoveryProvider.DiscoverAssemblyParts(Global.HostingEnvironment.ApplicationName);
            }
        }
    }
}