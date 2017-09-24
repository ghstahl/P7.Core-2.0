using System;
using System.IO;
using ZeroFormatter;

namespace P7.Store
{
    static public class PagingStateExtensions
    {
        public static byte[] SafeConvertFromBase64String(this string psString)
        {
            if (string.IsNullOrEmpty(psString))
                return null;
            return Convert.FromBase64String(psString);
        }
        public static string SafeConvertToBase64String(this byte[] bytes)
        {
            if (bytes == null)
                return null;
            return Convert.ToBase64String(bytes);
        }
        public static byte[] Serialize(this PagingState pagingState)
        {
            if (pagingState == null)
                return null;
            var bytes = ZeroFormatterSerializer.Serialize(pagingState);
            return bytes;
        }
        public static string SerializeToBase64String(this PagingState pagingState)
        {
            if (pagingState == null)
                return null;
            byte[] bytes = pagingState.Serialize();
            var psString = Convert.ToBase64String(bytes);
            return psString;
        }
        public static PagingState DeserializePageState(this byte[] bytes)
        {
            if (bytes == null)
                return new PagingState() {CurrentIndex = 0};
            var pagingState = ZeroFormatterSerializer.Deserialize<PagingState>(bytes);
            return pagingState;
        }
        public static PagingState DeserializePageStateFromBase64String(this string psString)
        {
            if (string.IsNullOrEmpty(psString))
                return new PagingState() { CurrentIndex = 0 };
            var bytes = Convert.FromBase64String(psString);
            PagingState pagingState = bytes.DeserializePageState();
            return pagingState;
        }
    }
}