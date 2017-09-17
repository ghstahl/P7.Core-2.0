using System;
using System.Linq;

namespace Hugo.Core.InMemory {
  public sealed class ItemsAreEqualsByReference<T> : IItemsAreEquals<T> {
    private readonly T itemToSearch;

    public ItemsAreEqualsByReference(T itemToSearch) {
      this.itemToSearch = itemToSearch;
    }

    public bool IsMatch(T item) {
      return this.itemToSearch.Equals(item)
          && ReferenceEquals(this.itemToSearch, item);
    }
  }
}