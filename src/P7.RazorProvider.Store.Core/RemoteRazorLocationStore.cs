using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using P7.Core.Utils;
using P7.RazorProvider.Store.Core.Interfaces;
using P7.RazorProvider.Store.Core.Models;
using Serilog;

namespace P7.RazorProvider.Store.Core
{
    public class RemoteRazorLocationStore : InMemoryRazorLocationStore, IRemoteRazorLocationStore
    {
        static Serilog.ILogger logger = Log.ForContext<RemoteRazorLocationStore>();

        // https://rawgit.com/ghstahl/P7/master/src/p7.external.spa/Areas/ExtSpa/views.json;
        public RemoteRazorLocationStore()
        {

        }

        public static RazorLocationViews FromJson(string json) =>
            JsonConvert.DeserializeObject<RazorLocationViews>(json, Settings);

        public static string ToJson(RazorLocationViews o) =>
            JsonConvert.SerializeObject((object) o, (JsonSerializerSettings) Settings);

        // JsonConverter stuff

        static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };

        public async Task LoadRemoteDataAsync(string url, string schema)
        {
            string content = await RemoteJsonFetch.GetRemoteJsonContentAsync(url, schema);

            if (content != null)
            {
                RazorLocationViews razorLocationViews;
                razorLocationViews = FromJson(content);
                var now = DateTime.UtcNow;

                var query = from item in razorLocationViews.Views
                    let c = new RazorLocation(item) {LastModified = now, LastRequested = now}
                    select c;

                var results = query.ToList();
                
                var lastRequetedTime = now.Subtract(new TimeSpan(0, 1, 0));
                foreach (var result in results)
                {
                    // make sure that there is no confusion that this is new
                    result.LastModified = now;
                    result.LastRequested = lastRequetedTime;
                }
                Insert(results);
            }
        }
    }
}