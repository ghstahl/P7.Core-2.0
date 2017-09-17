using System;
using System.Linq;
using System.Reflection;

namespace Hugo.Core.InMemory
{
    public sealed class ItemComparer
    {
        //todo change it to allow to create one comparer for the type so the Store can create the class once.
        public static IItemsAreEquals<T> CreateItemsComparer<T>(T item)
        {
            IItemsAreEquals<T> comparer = new ItemsAreEqualsByReference<T>(item);

            if (HasPrimaryKeyAttribute<T>())
            {
                comparer = new ItemsAreEqualsByKey<T>(item);
            }
            else if (HasIdProperty<T>())
            {
                comparer = new ItemsAreEqualsById<T>(item);
            }

            return comparer;
        }

        private static bool HasIdProperty<T>()
        {
            return typeof(T)
                .GetProperties()
                .Any(prop => string.Equals(prop.Name, "Id", StringComparison.OrdinalIgnoreCase));
        }

        private static bool HasPrimaryKeyAttribute<T>()
        {
            return typeof(T)
                .GetProperties()
                .Any(prop => prop.GetCustomAttributes(true).Any(attr => attr is PrimaryKeyAttribute));
        }
    }
}