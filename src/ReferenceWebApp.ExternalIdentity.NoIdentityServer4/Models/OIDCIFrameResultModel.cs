using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReferenceWebApp.Models
{
    public class OIDCIFrameResultModel
    {
        public string Error { get; set; }
        public Dictionary<string, string> OIDC { get; set; }
    }
}
