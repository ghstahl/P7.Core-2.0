using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Hugo.Core {

  public class UnitOfWork : IDisposable{

    public IDbCore Database { get; private set; }
    public UnitOfWork Parent { get; private set; }
    //public UnitOfWorkCollection SubUnits { get { return _subUnits; } }
    public bool IsTopLevel { get { return Parent == null; } }
    public List<IDbCommand> Commands { get; private set; }


    //UnitOfWorkCollection _subUnits;
    //bool _saveChangesCalled = false;

    public UnitOfWork(IDbCore dbCore) {
      Database = dbCore;

    }

    public void SaveChanges() {

    }


    //// Internal list implementation for managing sub-units:
    //public class UnitOfWorkCollection : ICollection<UnitOfWork> {

    //  List<UnitOfWork> _list;
    //  public UnitOfWork Parent { get; private set; }

    //  public UnitOfWorkCollection(UnitOfWork parent) {
    //    _list = new List<UnitOfWork>();
    //    Parent = parent;
    //  }

    //  public void Add(UnitOfWork item) {
    //    item.Parent = Parent;
    //    _list.Add(item);
    //  }

    //  public void Clear() {
    //    _list.Clear();
    //  }

    //  public bool Contains(UnitOfWork item) {
    //    return _list.Contains(item);
    //  }

    //  public void CopyTo(UnitOfWork[] array, int arrayIndex) {
    //    _list.CopyTo(array, arrayIndex);
    //  }

    //  public int Count {
    //    get { return _list.Count; }
    //  }

    //  public bool IsReadOnly {
    //    get { throw new NotImplementedException(); }
    //  }

    //  public bool Remove(UnitOfWork item) {
    //    return _list.Remove(item);
    //  }

    //  public IEnumerator<UnitOfWork> GetEnumerator() {
    //    return _list.GetEnumerator();
    //  }

    //  IEnumerator IEnumerable.GetEnumerator() {
    //    return _list.GetEnumerator();
    //  }
    //}
    
    
    
    
    
    private bool _disposed = false;
    public void Dispose() {
      Dispose(true);

      // Use SupressFinalize in case a subclass 
      // of this type implements a finalizer.
      GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing) {
      if (!_disposed) {
        if (disposing) {

        }

        // Indicate that the instance has been disposed.
        _disposed = true;
      }
    }
  }




}
