using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hugo.Core;
using Newtonsoft.Json;
using System.Data;
using Hugo.Utils;

namespace Hugo.Core {
  public abstract class DocumentStoreBase<T> : IDataStore<T> where T : new() {

    public IDbCore Database { get; protected set; }

    public bool KeyIsAutoIncrementing { get; set; }
    public string TableName { get; set; }

    private string _pkName;
    public string KeyName {
      get {
        if (string.IsNullOrWhiteSpace(_pkName)) {
          _pkName = this.GetKeyName();
        }
        return _pkName;
      }
      protected set {
        _pkName = value;
      }
    }

    private Type _keyType;
    public Type KeyType {
      get {
        if (_keyType == null) {
          _keyType = this.GetKeyType();
        }
        return _keyType;
      }
      protected set {
        _keyType = value;
      }
    }

    private PropertyInfo _keyProperty;
    protected virtual PropertyInfo KeyProperty {
      get {
        if (_keyProperty == null) {
          _keyProperty = this.GetKeyProperty();
          return _keyProperty;
        }
        return _keyProperty;
      }
      set {
        _keyProperty = value;
      }
    }

    protected virtual string DecideTableName() {
      if (String.IsNullOrWhiteSpace(this.TableName)) {
        this.TableName = Inflector.Pluralize(typeof(T).Name.ToLower());
      }
      return this.TableName;
    }

    //public PgDocumentStore(string connectionStringName, string tableName) {
    //  this.Database = new PgDbCore(connectionStringName);
    //  _keyProperty = this.GetKeyProperty();
    //  this.KeyIsAutoIncrementing = this.DecideKeyIsAutoIncrementing();
    //  TryLoadData();
    //}

    //public PgDocumentStore(string connectionStringName) {
    //  this.Database = new PgDbCore(connectionStringName);
    //  _keyProperty = this.GetKeyProperty();
    //  this.KeyIsAutoIncrementing = this.DecideKeyIsAutoIncrementing();
    //  TryLoadData();
    //}

    public DocumentStoreBase(IDbCore dbCore) {
      this.Database = dbCore;
      _keyProperty = this.GetKeyProperty();
      this.KeyIsAutoIncrementing = this.DecideKeyIsAutoIncrementing();
      TryLoadData();
    }

    public DocumentStoreBase(IDbCore dbCore, string tableName) {
      this.TableName = tableName;
      this.Database = dbCore;
      _keyProperty = this.GetKeyProperty();
      this.KeyIsAutoIncrementing = this.DecideKeyIsAutoIncrementing();
      TryLoadData();
    }



    public virtual void SetKeyValue(T item, object value) {
      var props = item.GetType().GetProperties();
      if (item is ExpandoObject) {
        var d = item as IDictionary<string, object>;
        d[this.KeyName] = value;
      } else {
        var pkProp = this.KeyProperty;
        var converted = Convert.ChangeType(value, pkProp.PropertyType);
        pkProp.SetValue(item, converted, null);
      }
    }

    protected virtual ExpandoObject SetDataForDocument(T item) {
      var json = JsonConvert.SerializeObject(item);
      var key = this.GetKeyValue(item);
      var expando = new ExpandoObject();
      var dict = expando as IDictionary<string, object>;

      dict[this.KeyName] = key;
      dict["body"] = json;
      return expando;
    }

    //public virtual List<T> TryLoadData() {
    //  var result = new List<T>();
    //  var tableName = DecideTableName();
    //  try {
    //    var sql = "select * from " + tableName;
    //    var data = Database.ExecuteDynamic(sql);
    //    //hopefully we have data
    //    foreach (var item in data) {
    //      //pull out the JSON
    //      var deserialized = JsonConvert.DeserializeObject<T>(item.body);
    //      result.Add(deserialized);
    //    }
    //  }
    //  catch (Npgsql.NpgsqlException x) {
    //    if (x.Message.Contains("does not exist")) {
    //      var sql = this.GetCreateTableSql();
    //      var added = Database.TransactDDL(Database.BuildCommand(sql));
    //      if (added == 0) {
    //        throw new InvalidProgramException("Document table not created");
    //      }
    //      TryLoadData();
    //    } else {
    //      throw;
    //    }
    //  }
    //  return result;
    //}

    protected virtual object GetKeyValue(T item) {
      var property = this.KeyProperty;
      return property.GetValue(item, null);
    }

    //protected virtual string GetCreateTableSql() {
    //  string tableName = this.DecideTableName();
    //  string pkName = this.GetKeyName();
    //  Type keyType = this.GetKeyType();
    //  bool isAuto = this.DecideKeyIsAutoIncrementing();

    //  string pkTypeStatement = "serial primary key";
    //  if (!isAuto) {
    //    pkTypeStatement = "int primary key";
    //  }
    //  if (keyType == typeof(string) || keyType == typeof(Guid)) {
    //    pkTypeStatement = "text primary key";
    //  }

    //  string sqlformat = @"create table {0} (id {1}, body json, created_at timestamptz)";
    //  return string.Format(sqlformat, tableName, pkTypeStatement);
    //}

    protected virtual bool DecideKeyIsAutoIncrementing() {
      var info = this.GetKeyProperty();
      var propertyType = info.PropertyType;

      // Key needs to be int, string:
      if (propertyType != typeof(int)
        && propertyType != typeof(string)) {
        throw new Exception("key must be either int or string");
      }
      // Decoration with an attribute overrides everything else:
      var attributes = info.GetCustomAttributes(false);
      if (attributes != null && attributes.Count() > 0) {
        var attribute = info.GetCustomAttributes(false).First(a => a.GetType() == typeof(PrimaryKeyAttribute));
        var pkAttribute = attribute as PrimaryKeyAttribute;
        if (pkAttribute.IsAutoIncrementing && propertyType == typeof(string)) {
          throw new Exception("A string key cannot be auto-incrementing. Set the 'IsAuto' Property on the PrimaryKey Attribute to False");
        }
        return pkAttribute.IsAutoIncrementing;
      }
      // Default for int is auto:
      if (propertyType == typeof(int)) {
        return true;
      }
      // Default for any other type is false, unless overridden with attribute:
      return false;
    }

    protected virtual string GetKeyName() {
      var info = this.GetKeyProperty();
      return info.Name;
    }

    protected virtual Type GetKeyType() {
      var info = this.GetKeyProperty();
      return info.PropertyType;
    }

    protected virtual PropertyInfo GetKeyProperty() {
      var myObject = new T();
      var myType = myObject.GetType();
      var myProperties = myType.GetProperties();
      string objectTypeName = myType.Name;
      PropertyInfo pkProperty = null;

      // Decoration with a [PrimaryKey] attribute overrides everything else:
      var foundProps = myProperties.Where(p => p.GetCustomAttributes(false)
        .Any(a => a.GetType() == typeof(PrimaryKeyAttribute)));

      if (foundProps != null && foundProps.Count() > 0) {
        // For now, more than one pk attribute is a problem:
        if (foundProps.Count() > 1) {
          var names = (from p in foundProps select p.Name).ToArray();
          string namelist = "";
          foreach (var pk in foundProps) {
            namelist = string.Join(",", names);
          }
          string keyIsAmbiguousMessageFormat = ""
            + "The key property for {0} is ambiguous between {1}. Please define a single key property.";
          throw new Exception(string.Format(keyIsAmbiguousMessageFormat, objectTypeName, namelist));
        } else {
          pkProperty = foundProps.ElementAt(0);
        }
      } else {
        // Is there a property named id (case irrelevant)?
        pkProperty = myProperties
          .FirstOrDefault(n => n.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase));
        if (pkProperty == null) {
          // Is there a property named TypeNameId (case irrelevant)?
          string findName = string.Format("{0}{1}", objectTypeName, "id");
          pkProperty = myProperties
            .FirstOrDefault(n => n.Name.Equals(findName, StringComparison.CurrentCultureIgnoreCase));
        }
        if (pkProperty == null) {
          string keyNotDefinedMessageFormat = ""
            + "No key property is defined on {0}. Please define a property which forms a unique key for objects of this type.";
          throw new Exception(string.Format(keyNotDefinedMessageFormat, objectTypeName));
        }
      }
      return pkProperty;
    }


    public abstract IEnumerable<IDbCommand> CreateInsertCommands(IEnumerable<T> items);
    public abstract IEnumerable<IDbCommand> CreateUpdateCommands(IEnumerable<T> items);
    public abstract IEnumerable<IDbCommand> CreateDeleteCommands(IEnumerable<T> items);
    public abstract IDbCommand CreateDeleteAllCommand();
    protected abstract string GetCreateTableSql();
    public abstract List<T> TryLoadData();

    public virtual int Add(T item) {
      return this.Add(new T[] { item });
    }

    public virtual int Add(IEnumerable<T> items) {
      var commands = CreateInsertCommands(items);
      return Database.Transact(commands.ToArray());
    }

    public virtual int Update(T item) {
      return this.Update(new T[] { item });
    }

    public virtual int Update(IEnumerable<T> items) {
      var commands = CreateUpdateCommands(items);
      return Database.Transact(commands.ToArray());
    }

    public virtual int Delete(T item) {
      return this.Delete(new T[] { item });
    }

    public virtual int Delete(IEnumerable<T> items) {
      var commands = CreateDeleteCommands(items);
      return Database.Transact(commands.ToArray());
    }

    public virtual int DeleteAll() {
      var command = CreateDeleteAllCommand();
      return Database.Transact(command);
    }
  }
}
