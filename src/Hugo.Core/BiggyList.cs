using System;
using System.Collections.Generic;
using System.Linq;

namespace Hugo.Core {
  public class BiggyList<T> : ICollection<T> where T : new() {

    public BiggyList(IDataStore<T> store, bool inMemory = false) {
      _items = new List<T>();
      this.InMemory = inMemory;
      _store = store;
      if (UpdateLiveDataAllowed) {
        _items = _store.TryLoadData();
      }
    }

    public BiggyList() {
      this.InMemory = true;
      _items = new List<T>();
      _store = null;
      if (UpdateLiveDataAllowed) {
        _items = _store.TryLoadData();
      }
    }

    protected List<T> _items = null;
    protected IDataStore<T> _store;

    public virtual bool InMemory { get; protected set; }
    public IDataStore<T> Store { get { return _store; } }

    public event EventHandler ItemRemoved;
    public event EventHandler ItemsRemoved;
    public event EventHandler ItemAdded;
    public event EventHandler ItemsAdded;
    public event EventHandler ItemUpdated;
    public event EventHandler ItemsUpdated;
    public event EventHandler Changed;
    public event EventHandler Loaded;
    public event EventHandler Saved;

    protected virtual bool UpdateLiveDataAllowed {
      get {
        if (this.InMemory == false && this.Store != null) {
          return true;
        }
        return false;
      }
    }

    public virtual int Update(T item) {
      if (this.UpdateLiveDataAllowed) {
        this.Store.Update(item);
      }
      var index = _items.IndexOf(item);
      if (index > -1) {
        _items.RemoveAt(index);
        _items.Insert(index, item);
      }
      FireUpdatedEvents(item);
      FireChangedEvents();
      return 1;
    }

    public virtual int Update(IEnumerable<T> items) {
      if (this.UpdateLiveDataAllowed) {
        this.Store.Update(items);
      }
      // We need to do this because a client call using Linq against
      // the BiggyList may yield results from an enumerator, and this will
      // modify the collection on which the enumerator is based:
      var itemsToUpdate = items.ToList();
      foreach (var item in itemsToUpdate) {
        var index = _items.IndexOf(item);
        if (index > -1) {
          _items.RemoveAt(index);
          _items.Insert(index, item);
        }
      }
      FireUpdatedEvents(items);
      FireChangedEvents();
      return items.Count();
    }

    public virtual void Add(T item) {
      if (this.UpdateLiveDataAllowed) {
        this.Store.Add(item);
      }
      _items.Add(item);
      FireInsertedEvents(item);
      FireChangedEvents();
    }

    public void Add(IEnumerable<T> items) {
      if (this.UpdateLiveDataAllowed) {
        this.Store.Add(items);
      }
      _items.AddRange(items);
      FireInsertedEvents(items);
      FireChangedEvents();
    }

    public virtual void Clear() {
      if (this.UpdateLiveDataAllowed) {
        this.Store.DeleteAll();
      }
      _items.Clear();
      FireChangedEvents();
    }

    public virtual bool Contains(T item) {
      return _items.Contains(item);
    }

    public virtual void CopyTo(T[] array, int arrayIndex) {
      _items.CopyTo(array, arrayIndex);
    }

    public virtual int Count {
      get { return _items.Count; }
    }

    public virtual bool IsReadOnly {
      get { return false; }
    }

    public virtual bool Remove(T item) {
      if (this.UpdateLiveDataAllowed) {
        this.Store.Delete(item);
      }
      var removed = _items.Remove(item);
      FireRemovedEvents(item);
      FireChangedEvents();
      return removed;
    }

    public virtual int Remove(IEnumerable<T> items) {
      if (this.UpdateLiveDataAllowed) {
        this.Store.Delete(items);
      }
      // We need to do this because a client call using Linq against
      // the BiggyList may yield results from an enumerator, and this will
      // modify the collection on which the enumerator is based:
      var itemsToRemove = items.ToList();
      foreach (var item in itemsToRemove) {
        _items.Remove(item);
      }
      FireRemovedEvents(items);
      FireChangedEvents();
      return items.Count();
    }

    public IEnumerator<T> GetEnumerator() {
      return _items.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return _items.GetEnumerator();
    }

    protected virtual void FireChangedEvents() {
      if (this.Changed != null) {
        var args = new BiggyEventArgs<T>();
        args.Items = _items;
        this.ItemRemoved.Invoke(this, args);
      }
    }

    protected virtual void FireRemovedEvents(T item) {
      if (this.ItemRemoved != null) {
        var args = new BiggyEventArgs<T>();
        args.Item = item;
        this.ItemRemoved.Invoke(this, args);
      }
    }

    protected virtual void FireRemovedEvents(IEnumerable<T> items) {
      if (this.ItemsRemoved != null) {
        var args = new BiggyEventArgs<T>();
        args.Items = items.ToList();
        this.ItemsRemoved.Invoke(this, args);
      }
    }

    protected virtual void FireInsertedEvents(T item) {
      if (this.ItemAdded != null) {
        var args = new BiggyEventArgs<T>();
        args.Item = item;
        this.ItemAdded.Invoke(this, args);
      }
    }

    protected virtual void FireInsertedEvents(IEnumerable<T> items) {
      if (this.ItemsAdded != null) {
        var args = new BiggyEventArgs<T>();
        args.Items = items.ToList();
        this.ItemsAdded.Invoke(this, args);
      }
    }

    protected virtual void FireUpdatedEvents(T item) {
      if (this.ItemUpdated != null) {
        var args = new BiggyEventArgs<T>();
        args.Item = item;
        this.ItemUpdated.Invoke(this, args);
      }
    }

    protected virtual void FireUpdatedEvents(IEnumerable<T> items) {
      if (this.ItemsUpdated != null) {
        var args = new BiggyEventArgs<T>();
        args.Items = items.ToList();
        this.ItemsUpdated.Invoke(this, args);
      }
    }

    protected virtual void FireLoadedEvents() {
      if (this.Loaded != null) {
        var args = new BiggyEventArgs<T>();
        args.Items = _items;
        this.Loaded.Invoke(this, args);
      }
    }

    protected virtual void FireSavedEvents() {
      if (this.Saved != null) {
        var args = new BiggyEventArgs<T>();
        args.Items = _items;
        this.Saved.Invoke(this, args);
      }
    }
  }
}