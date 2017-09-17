using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P7.IdentityServer4.Common.Constants
{
    public static class P7_Constants
    {
        public const string @namespace = "p7:";
    }
    public static class ClaimTypes
    {
        public const string AccountGuid = P7_Constants.@namespace + "accountguid";
        public const string UserGuid = P7_Constants.@namespace + "userguid";
        public const string ClientRequestNameValueCollection = P7_Constants.@namespace + "crnvc";
    }
}
