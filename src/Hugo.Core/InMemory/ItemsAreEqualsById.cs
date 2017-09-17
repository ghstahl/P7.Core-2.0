using System;
using System.Linq;
using System.Reflection;

namespace Hugo.Core.InMemory
{
    public sealed class ItemsAreEqualsById<T> : IItemsAreEquals<T>
    {
        private readonly object keyValue;
        private readonly PropertyInfo property;

        public ItemsAreEqualsById(T itemToSearch)
        {
            this.property = GetIdProperty(itemToSearch);
            this.keyValue = property.GetValue(itemToSearch, null);
        }

        public bool IsMatch(T item)
        {
            var itemValue = this.property.GetValue(item, null);

            return this.keyValue.Equals(itemValue);
        }

        private static PropertyInfo GetIdProperty(T itemToSearch)
        {
            return itemToSearch
                .GetType()
                .GetProperties()
                .Single(x => string.Equals(x.Name, "Id", StringComparison.OrdinalIgnoreCase));
        }

        private string FindKeyByAttribute()
        {
            var property = typeof(T)
                .GetProperties()
                .First(prop =>
                    (prop.GetCustomAttributes(true).Any(attr =>
                       attr is PrimaryKeyAttribute)));

            return property.Name;
        }
    }
}