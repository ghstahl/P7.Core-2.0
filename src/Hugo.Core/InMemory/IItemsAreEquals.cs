using System;
using System.Linq;

namespace Hugo.Core.InMemory {
  public interface IItemsAreEquals<T> {
    bool IsMatch(T item);
  }
}