using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Hugo.Core {
  public abstract class RelationalStoreBase<T> : IDataStore<T> where T : new() {
    public DBTableMapping TableMapping { get; set; }

    public IDbCore Database { get; set; }

    public RelationalStoreBase(IDbCore dbCore) {
      this.Database = dbCore;
      this.TableMapping = this.Database.getTableMappingFor<T>();
    }
    public abstract List<T> TryLoadData();

    public abstract IEnumerable<IDbCommand> CreateInsertCommands(IEnumerable<T> items);
    public abstract IEnumerable<IDbCommand> CreateUpdateCommands(IEnumerable<T> items);
    public abstract IEnumerable<IDbCommand> CreateDeleteCommands(IEnumerable<T> items);
    public abstract IDbCommand CreateDeleteAllCommand();

    public virtual int Add(IEnumerable<T> items) {
      var commands = CreateInsertCommands(items);
      return Database.Transact(commands.ToArray());
    }

    public virtual int Add(T item) {
      return Add(new T[] { item });
    }

    public virtual int Update(IEnumerable<T> items) {
      var commands = CreateUpdateCommands(items);
      return Database.Transact(commands.ToArray());
    }

    public virtual int Update(T item) {
      return this.Update(new T[] { item });
    }

    public virtual int Delete(IEnumerable<T> items) {
      var commands = CreateDeleteCommands(items);
      return Database.Transact(commands.ToArray());
    }

    public virtual int Delete(T item) {
      return this.Delete(new T[] { item });
    }

    public virtual int DeleteAll() {
      return Database.Transact(CreateDeleteAllCommand());
    }


    public bool KeyIsAutoIncrementing {
      get {
        if (this.TableMapping.PrimaryKeyMapping[0].IsAutoIncementing) {
          return true;
        }
        return false;
      }
    }

    protected virtual U MapReaderToObject<U>(IDataReader reader) where U : new() {
      var item = new U();
      var props = item.GetType().GetProperties();
      foreach (var property in props) {
        if (this.TableMapping.ColumnMappings.ContainsPropertyName(property.Name)) {
          string mappedColumn = this.TableMapping.ColumnMappings.FindByProperty(property.Name).ColumnName;
          int ordinal = reader.GetOrdinal(mappedColumn);
          var val = reader.GetValue(ordinal);
          if (val.GetType() != typeof(DBNull)) {
            property.SetValue(item, Convert.ChangeType(reader.GetValue(ordinal), property.PropertyType), null);
          }
        }
      }
      return item;
    }
  }
}