using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace P7.External.SPA.Models
{
    public class SectionValue
    {
        public HtmlString Value { get; set; }
    }
    public class ClaimType
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
