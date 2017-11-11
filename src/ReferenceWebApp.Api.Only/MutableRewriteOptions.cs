using System.Collections.Generic;
using Microsoft.AspNetCore.Rewrite;

namespace ReferenceWebApp
{
    public class MutableRewriteOptions : RewriteOptions
    {
        //
        // Summary:
        //     A list of Microsoft.AspNetCore.Rewrite.IRule that will be applied in order upon
        //     a request.
        public new IList<IRule> Rules { get; set; }
    }
}