using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace P7.External.SPA.Core
{

    public class ExternalSPARecord
    {
        [JsonProperty("renderTemplate")]
        public string RenderTemplate { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("requireAuth")]
        public bool RequireAuth { get; set; }

        [JsonProperty("view")]
        public string View { get; set; }
    }
    public class SpaRecords
    {
        [JsonProperty("spas")]
        public ExternalSPARecord[] Spas { get; set; }
    }

    public interface IExternalSPAStore
    {
        ExternalSPARecord GetRecord(string key);
        void AddRecord(ExternalSPARecord record);
        void RemoveRecord(string key);
        IEnumerable<ExternalSPARecord> GetRecords();
    }
}
