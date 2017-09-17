using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Hugo.Extensions {
  public static class ObjectExtensions {
    /// <summary>
    /// Turns the object into an ExpandoObject
    /// </summary>
    public static dynamic ToExpando(this object o) {
      var result = new ExpandoObject();
      var d = result as IDictionary<string, object>; //work with the Expando as a Dictionary
      if (o.GetType() == typeof(ExpandoObject)) return o; //shouldn't have to... but just in case
      if (o.GetType() == typeof(NameValueCollection) || o.GetType().GetTypeInfo().IsSubclassOf(typeof(NameValueCollection))) {
        var nv = (NameValueCollection)o;
        nv.Cast<string>().Select(key => new KeyValuePair<string, object>(key, nv[key])).ToList().ForEach(i => d.Add(i));
      } else {
        var props = o.GetType().GetProperties();
        foreach (var item in props) {
          if (item.CanWrite) {
            d.Add(item.Name, item.GetValue(o, null));
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Turns the object into a Dictionary
    /// </summary>
    public static IDictionary<string, object> ToDictionary(this object thingy) {
      return (IDictionary<string, object>)thingy.ToExpando();
    }
  }
}