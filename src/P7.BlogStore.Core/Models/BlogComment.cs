using System;
using Newtonsoft.Json;
using P7.Core.Utils;
using P7.Store;

namespace P7.BlogStore.Core
{
    public class BlogComment : DocumentBase
    {
        [JsonIgnore]
        public bool EnableDeepCompare { get; set; }

        public string Comment { get; set; }
        public DateTime TimeStamp { get; set; }

        public BlogComment()
        {
            EnableDeepCompare = false;
        }

        public BlogComment(BlogComment doc)
        {
            this.Id = doc.Id;
            this.Comment = doc.Comment;
        }


        public override bool Equals(object obj)
        {
            return EnableDeepCompare ? DeepEquals(obj) : ShallowEquals(obj);
        }
        public bool DeepEquals(object obj)
        {
            var other = obj as BlogComment;
            if (other == null)
            {
                return false;
            }

            if (!Id.IsEqual(other.Id))
            {
                return false;
            }
            if (!TimeStamp.IsEqual(other.TimeStamp))
            {
                return false;
            }
            if (!Comment.IsEqual(other.Comment))
            {
                return false;
            }
            return true;
        }
        public bool ShallowEquals(object obj)
        {
            var other = obj as BlogComment;
            if (other == null)
            {
                return false;
            }

            if (!Id.IsEqual(other.Id))
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}