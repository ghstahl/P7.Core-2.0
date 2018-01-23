using System;
using Newtonsoft.Json;
using ZeroFormatter;

namespace P7.RazorProvider.Store.Core.Models
{
    [ZeroFormattable]
    public class RazorLocation : IComparable
    {
        [JsonProperty("location")]
        [Index(0)]
        public virtual string Location { get; set; }

        [IgnoreFormat]
        public string Id => Location;

        [JsonProperty("content")]
        [Index(1)]
        public virtual string Content { get; set; }
        [JsonProperty("lastModified")]
        [Index(2)]
        public virtual DateTime LastModified { get; set; }
        [JsonProperty("lastRequested")]
        [Index(3)]
        public virtual DateTime LastRequested { get; set; }

       
        [Index(4)]
        public virtual byte[] ByteContent { get; set; }


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