using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReferenceWebApp.Models
{
    public class iFrameSilentSigninModel
    {
        public string ReturnUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string Prompt { get; set; }
        public string Provider { get; set; }
    }
}
