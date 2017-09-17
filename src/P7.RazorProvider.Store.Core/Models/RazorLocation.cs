using System;
using Newtonsoft.Json;

namespace P7.RazorProvider.Store.Core.Models
{
    public class RazorLocation : IComparable
    {
        [JsonProperty("location")]
        public string Location { get; set; }

        public string Id => Location;

        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("lastModified")]
        public DateTime LastModified { get; set; }
        [JsonProperty("lastRequested")]
        public DateTime LastRequested { get; set; }

      
        public RazorLocation()
        {
        }

        public RazorLocation(RazorLocation doc)
        {
            
            this.Location = doc.Location;
            this.Content = doc.Content;
            this.LastModified = doc.LastModified;
            this.LastRequested = doc.LastRequested;
        }
       
        public override bool Equals(object obj)
        {
            return  ShallowEquals(obj);
        }
        public bool ShallowEquals(object obj)
        {
            var other = obj as RazorLocation;
            if (other == null)
            {
                return false;
            }

            return true;
        }

        public int CompareTo(object obj)
        {
            if (Equals(obj))
                return 0;
            return -1;
        }
    }
}