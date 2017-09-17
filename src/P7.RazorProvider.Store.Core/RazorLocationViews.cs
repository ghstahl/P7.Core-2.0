using System.Collections.Generic;
using Newtonsoft.Json;
using P7.RazorProvider.Store.Core.Models;

namespace P7.RazorProvider.Store.Core
{
    public class RazorLocationViews
    {
        [JsonProperty("views")]
        public RazorLocation[] Views { get; set; }
    }

}