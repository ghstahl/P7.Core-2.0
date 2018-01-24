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
    public partial class FilesManifest
    {
        [JsonProperty("files")]
        public string[] Files { get; set; }
    }
    public partial class FilesManifest
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
        public static FilesManifest FromJson(string json) => JsonConvert.DeserializeObject<FilesManifest>(json, Settings);
    }

    public class RemoteFileSyncTask : IScheduledTask
    {
        static Serilog.ILogger logger = Log.ForContext<RemoteFileSyncTask>();
        private IConfiguration _config { get; set; }
        public RemoteFileSyncTask(IConfiguration config)
        {
            _config = config;
        }
        public string Schedule => "*/1 * * * *";  // every 1 minute
        private async Task<FilesManifest> GetRemoteFilesConfigAsync(string url, bool validateSchema = false)
        {
            //https://rawgit.com/ghstahl/P7.Core-2.0.RemoteData/master/spas/AngularServerSide/files.json
            try
            {
                var schemaUrl = url.Replace(".json", ".schema.json");
                var schema = await RemoteJsonFetch.GetRemoteJsonContentAsync(schemaUrl);

                string content = await RemoteJsonFetch.GetRemoteJsonContentAsync(url, schema);
                FilesManifest filesManifest;
                filesManifest = FilesManifest.FromJson(content);

                return filesManifest;
            }
            catch (Exception e)
            {
                logger.Fatal("Exception Caught:{0}", e.Message);
            }
            return null;
        }
        public async Task Invoke(CancellationToken cancellationToken)
        {
            var filesConfig = await GetRemoteFilesConfigAsync(
                "https://rawgit.com/ghstahl/P7.Core-2.0.RemoteData/master/spas/AngularServerSide/files.json");

        }
    }

    public class RemoteRazorLocationStoreTask : IScheduledTask
    {
        private IRemoteRazorLocationStore RemoteRazorLocationStore { get; set; }
        static Serilog.ILogger logger = Log.ForContext<RemoteRazorLocationStoreTask>();
        private IConfiguration _config { get; set; }
        public RemoteRazorLocationStoreTask(IConfiguration config,IRemoteRazorLocationStore store)
        {
            RemoteRazorLocationStore = store;
            _config = config;
        }
        public string Schedule => "*/1 * * * *";  // every 1 minute



        private async Task<RemoteUrls> GetRemoteUrlsAsync(string url, bool validateSchema = false)
        {
            try
            {
                var schemaUrl = url.Replace(".json", ".schema.json");
                var schema = await RemoteJsonFetch.GetRemoteJsonContentAsync(schemaUrl);

                string content = await RemoteJsonFetch.GetRemoteJsonContentAsync(url, schema);
                RemoteUrls remoteViewUrls;
                remoteViewUrls = ExternUrlsOptionConvert.FromJson(content);

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
            var appConfig = new ExternalUrlsOptions();
            _config.GetSection("externalViews").Bind(appConfig);

            var urlViewSchema = await RemoteJsonFetch.GetRemoteJsonContentAsync(appConfig.UrlViewSchema);

            var remoteViewUrls = await GetRemoteUrlsAsync(appConfig.Urls,true);

            foreach (var url in remoteViewUrls.Urls)
            {
                await RemoteRazorLocationStore.LoadRemoteDataAsync(url, urlViewSchema);
            }
        }
    }
}
 