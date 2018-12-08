using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using P7.Core.Scheduler.Scheduling;
using P7.Core.Utils;
using P7.HealthCheck.Core;
using P7.RazorProvider.Store.Core;
using P7.RazorProvider.Store.Core.Interfaces;
using P7.RazorProvider.Store.Core.Models;
using Serilog;

namespace P7.External.SPA.Scheduler
{
    public partial class MetaData
    {
        [JsonProperty("timeStamp")]
        public string TimeStamp { get; set; }
    }

    public partial class FilesManifest
    {
        [JsonProperty("metaData")]
        public MetaData MetaData { get; set; }

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
        private IHealthCheckStore HealthCheckStore { get; set; }
        private static FilesManifest FilesManifest { get; set; }
        static Serilog.ILogger logger = Log.ForContext<RemoteFileSyncTask>();
        private IHostingEnvironment _env { get; set; }
        private IConfiguration _config { get; set; }
        public RemoteFileSyncTask(IConfiguration config,IHostingEnvironment env, IHealthCheckStore healthCheckStore)
        {
            HealthCheckStore = healthCheckStore;
            _env = env;
            _config = config;
        }
        public string Schedule => "*/1 * * * *";  // every 1 minute
        private async Task<FilesManifest> GetRemoteFilesConfigAsync(string url, bool validateSchema = false)
        {
            //https://cdn.jsdelivr.net/gh/ghstahl/P7.Core-2.0.RemoteData/spas/AngularServerSide/files.json
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
        public static async Task<byte[]> GetRemoteContentAsync(string url)
        {
            var byteResult = await RemoteFetch.FetchAsync(url, new WebRequestInit() {  });
            return byteResult;
        }
        public async Task Invoke(CancellationToken cancellationToken)
        {
            var key = "remote-file-sync";
            var currentHealth = await HealthCheckStore.GetHealthAsync(key);
            if (currentHealth == null)
            {
                currentHealth = new HealthRecord()
                {
                    Healty = false,
                    State = null
                };
                await HealthCheckStore.SetHealthAsync(key, currentHealth);
            }

            var filesConfig = await GetRemoteFilesConfigAsync(
                "https://cdn.jsdelivr.net/gh/ghstahl/P7.Core-2.0.RemoteData/spas/AngularServerSide/files.json");
            bool newFile = true;
            if (FilesManifest != null)
            {
                newFile = FilesManifest.MetaData.TimeStamp != filesConfig.MetaData.TimeStamp;
            }

            if (newFile)
            {
                FilesManifest = filesConfig;

                var outDir = Path.Combine(_env.WebRootPath, "AngularServerSide/");
                Directory.CreateDirectory(outDir);
                foreach (var file in filesConfig.Files)
                {
                    var fileUrl = $"https://cdn.jsdelivr.net/gh/ghstahl/P7.Core-2.0.RemoteData/spas/AngularServerSide/{file}";
                    var fileData = await GetRemoteContentAsync(fileUrl);
                    var finalPath = Path.Combine(outDir, file);
                    var index = finalPath.LastIndexOf('/');
                    var finalDir = finalPath.Substring(0, index);
                    Directory.CreateDirectory(finalDir);
                    try
                    {
                        File.WriteAllBytes(finalPath, fileData);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            if (!currentHealth.Healty)
            {
                // once loaded we are good
                currentHealth = new HealthRecord()
                {
                    Healty = true,
                    State = null
                };
                await HealthCheckStore.SetHealthAsync(key, currentHealth);
            }
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
 