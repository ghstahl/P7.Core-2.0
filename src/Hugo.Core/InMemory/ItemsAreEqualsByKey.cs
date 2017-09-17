using System;
using System.Linq;
using System.Reflection;

namespace Hugo.Core.InMemory {
  public sealed class ItemsAreEqualsByKey<T> : IItemsAreEquals<T> {
    private readonly object keyValue;
    private readonly PropertyInfo property;

    public ItemsAreEqualsByKey(T itemToSearch) {
      var keyName = this.FindKeyByAttribute();
      this.property = itemToSearch.GetType().GetProperty(keyName);
    }

    public ItemsAreEqualsByKey(T itemToSearch, string keyName) {
      this.property = itemToSearch.GetType().GetProperty(keyName);
      this.keyValue = property.GetValue(itemToSearch, null);
    }

    public bool IsMatch(T item) {
      var itemValue = this.property.GetValue(item, null);

      return this.keyValue == itemValue;
    }

    private string FindKeyByAttribute() {
      var property = typeof(T)
          .GetProperties()
          .First(prop =>
              (prop.GetCustomAttributes(true).Any(attr =>
                 attr is PrimaryKeyAttribute)));

      return property.Name;
    }
  }
}