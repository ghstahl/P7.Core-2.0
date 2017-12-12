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
 