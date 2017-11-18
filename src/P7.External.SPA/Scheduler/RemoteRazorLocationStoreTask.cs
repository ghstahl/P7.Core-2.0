using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using P7.Core.Scheduler.Scheduling;
using P7.Core.Utils;
using P7.RazorProvider.Store.Core;
using P7.RazorProvider.Store.Core.Interfaces;
using P7.RazorProvider.Store.Core.Models;
using Serilog;

namespace P7.External.SPA.Scheduler
{

    public class ExternalViewOptions
    {
        [JsonProperty("urls")]
        public string Urls { get; set; }

        [JsonProperty("urlViewSchema")]
        public string UrlViewSchema { get; set; }
        
    }
    public partial class RemoteViewUrls
    {
        [JsonProperty("urls")]
        public string[] Urls { get; set; }
    }


    public class RemoteRazorLocationStoreTask : IScheduledTask
    {
    //    private const string Url = "https://rawgit.com/ghstahl/P7.Core-2.0/master/src/P7.External.SPA/Areas/ExtSpa/views.json";
        private IRemoteRazorLocationStore RemoteRazorLocationStore { get; set; }
        static Serilog.ILogger logger = Log.ForContext<RemoteRazorLocationStore>();
        private IConfiguration _config { get; set; }
        public RemoteRazorLocationStoreTask(IConfiguration config,IRemoteRazorLocationStore store)
        {
            RemoteRazorLocationStore = store;
            _config = config;
        }
        public string Schedule => "*/1 * * * *";  // every 1 minute
        public static RemoteViewUrls FromJson(string json) => JsonConvert.DeserializeObject<RemoteViewUrls>(json, Settings);

        public static string ToJson(RemoteViewUrls o) => JsonConvert.SerializeObject((object)o, (JsonSerializerSettings)Settings);
        // JsonConverter stuff

        static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };


        private async Task<RemoteViewUrls> GetRemoteViewUrlsAsync(string url, bool validateSchema = false)
        {
            try
            {
                var schemaUrl = url.Replace(".json", ".schema.json");
                var schema = await RemoteJsonFetch.GetRemoteJsonContentAsync(schemaUrl);

                string content = await RemoteJsonFetch.GetRemoteJsonContentAsync(url, schema);
                RemoteViewUrls remoteViewUrls;
                remoteViewUrls = FromJson(content);

                return remoteViewUrls;
            }
            catch (Exception e)
            {
                logger.Fatal("Exception Caught:{0}", e.Message);
            }
            return null;
        }

        public async Task Invoke(CancellationToken cancellationToken)
        {
            var appConfig = new ExternalViewOptions();
            _config.GetSection("externalViews").Bind(appConfig);

            var urlViewSchema = await RemoteJsonFetch.GetRemoteJsonContentAsync(appConfig.UrlViewSchema);

            var remoteViewUrls = await GetRemoteViewUrlsAsync(appConfig.Urls,true);

            foreach (var url in remoteViewUrls.Urls)
            {
                await RemoteRazorLocationStore.LoadRemoteDataAsync(url, urlViewSchema);
            }
        }
    }
}
 