using System;
using System.Collections.Generic;
using System.Text;
using Hugo.Data.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace P7.HugoStore.Core
{
    public class P7JsonStore<T> : JsonStore<T> where T : class, new()
    {
        public P7JsonStore(string folderStorage, string databaseName, string collection):base(folderStorage,databaseName,collection)
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Converters = new List<JsonConverter>()
                {
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter()
                }
            };
        }
    }
}
