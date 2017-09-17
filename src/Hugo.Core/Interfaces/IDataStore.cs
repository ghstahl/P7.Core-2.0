using System;
using System.Collections.Generic;
using System.Data;

namespace Hugo.Core {
  public interface IDataStore<T>
      where T : new() {
    int Add(IEnumerable<T> items);
    int Add(T item);
    int Delete(IEnumerable<T> items);
    int Delete(T item);
    int DeleteAll();
    int Update(System.Collections.Generic.IEnumerable<T> items);
    int Update(T item);
    List<T> TryLoadData();
  }
}