using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P7.Core.Reflection;

namespace P7.Core.Utils
{
    public static class EqualExtensions
    {
        public static bool SafeEquals<T>(this T a, T b)
        {

            if (a == null && b == null)
                return true;
            if (a != null && b != null)
            {
                if (!a.IsGenericList())
                {
                    return a.Equals(b);
                }
            }

            return false;
        }
        public static bool SafeListEquals<T>(this List<T> a, List<T> b)
        {
            if (a == null && b == null)
                return true;
            if (a != null && b != null)
            {
                IEnumerable<T> difference = a.Except(b);
                var equals = !difference.Any();
                return equals;
            }
            return false;
        }
        public static bool SafeListEquals<T>(this IList<T> a, IList<T> b)
        {
            if (a == null && b == null)
                return true;
            if (a != null && b != null)
            {
                var difference = a.Except(b);
                var equals = !difference.Any();
                return equals;
            }
            return false;
        }
    }
}
