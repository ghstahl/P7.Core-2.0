using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using P7.Core.Utils;
using P7.SimpleDocument.Store;
using P7.Store;

namespace P7.BlogStore.Core
{

 
    public class Blog : IComparable
    {
        [JsonIgnore]
        public bool EnableDeepCompare { get; set; }

        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        public string Data { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public Blog()
        {
            EnableDeepCompare = false;
        }

        public Blog(Blog doc)
        {
            
            this.Categories = doc.Categories;
            this.Data = doc.Data;
            this.Tags = doc.Tags;
            this.TimeStamp = doc.TimeStamp;
            this.Summary = doc.Summary;
            this.Title = doc.Title;
        }
       
        public override bool Equals(object obj)
        {
            return EnableDeepCompare ? DeepEquals(obj) : ShallowEquals(obj);
        }
        public bool ShallowEquals(object obj)
        {
            var other = obj as Blog;
            if (other == null)
            {
                return false;
            }

            return true;
        }
        public bool DeepEquals(object obj)
        {
            var other = obj as Blog;
            if (other == null)
            {
                return false;
            }

            var bothNull = Categories == null && other.Categories == null;
            var bothNotNull = Categories != null && other.Categories != null;

            if (bothNotNull)
            {
                if (Categories.Except(other.Categories).Any())
                    return false;
            }
            else if (!bothNull)
            {
                return false;
            }

            bothNull = Tags == null && other.Tags == null;
            bothNotNull = Tags != null && other.Tags != null;
            if (bothNotNull)
            {
                if (Tags.Except(other.Tags).Any())
                    return false;
            }
            else if (!bothNull)
            {
                return false;
            }
            
            if (!Data.IsEqual(other.Data))
            {
                return false;
            }
            if (!TimeStamp.IsEqual(other.TimeStamp))
            {
                return false;
            }
            if (!Summary.IsEqual(other.Summary))
            {
                return false;
            }
            if (!Title.IsEqual(other.Title))
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