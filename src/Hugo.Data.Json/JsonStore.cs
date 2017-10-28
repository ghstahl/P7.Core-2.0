using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using Hugo.Core;
using Hugo.Extensions;
using Newtonsoft.Json;
using System.Threading;
using Hugo.Utils;
using System.Text;

namespace Hugo.Data.Json
{
	public class JsonStore<T> : IDataStore<T> where T : class, new()
	{
        public JsonSerializerSettings JsonSerializerSettings { get; set; }
        public bool KeyIsAutoIncrementing { get; set; }
		public string TableName { get; set; }
		public string DbDirectory { get { return this.Database.DbDirectory; } set { this.DbDirectory = value; } }

		public string DbFileName
		{
			get { return this.TableName + ".json"; }
		}

		public string DbPath
		{
			get
			{
				return Path.Combine(DbDirectory, DbFileName);
			}
		}

		// This should never be returned directly to the client, because
		// iteration/modification issues.
		internal List<T> _items;

		private string _pkName;

		public string KeyName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_pkName))
				{
					_pkName = this.GetKeyName();
				}
				return _pkName;
			}
		}

		private Type _keyType;

		public Type KeyType
		{
			get
			{
				if (_keyType == null)
				{
					_keyType = this.GetKeyType();
				}
				return _keyType;
			}
		}

		private PropertyInfo _keyProperty;

		protected virtual PropertyInfo KeyProperty
		{
			get
			{
				if (_keyProperty == null)
				{
					_keyProperty = this.GetKeyProperty();
					return _keyProperty;
				}
				return _keyProperty;
			}
		}

		public bool IsFlushing { get; set; }

		public JsonDbCore Database { get; set; }

		public JsonStore()
		{
			this.DecideTableName();
			Database = new JsonDbCore();
			IsFlushing = false;
			this.TryLoadData();
		}

		public JsonStore(string tableName)
		{
			this.TableName = tableName;
			Database = new JsonDbCore();
			IsFlushing = false;
			this.TryLoadData();
		}

		public JsonStore(JsonDbCore dbCore)
		{
			this.DecideTableName();
			Database = dbCore;
			IsFlushing = false;
			this.TryLoadData();
		}

		public JsonStore(string dbName, string tableName)
		{
			Database = new JsonDbCore(dbName);
			this.TableName = tableName;
			IsFlushing = false;
			this.TryLoadData();
		}

		public JsonStore(string dbDirectory, string dbName, string tableName)
		{
			Database = new JsonDbCore(dbDirectory, dbName);
			this.TableName = tableName;
			IsFlushing = false;
			this.TryLoadData();
		}

		protected virtual string DecideTableName()
		{
			if (String.IsNullOrWhiteSpace(this.TableName))
			{
				this.TableName = Inflector.Pluralize(typeof(T).Name.ToLower());
			}
			return this.TableName;
		}

		private string GetDefaultDirectory()
		{
			return this.Database.GetDefaultDirectory();
		}

		public virtual int Add(T item)
		{
			_items.Add(item);
			if (this.FlushToDisk())
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		public virtual int Add(IEnumerable<T> items)
		{
			_items.AddRange(items);
			if (this.FlushToDisk())
			{
				return items.Count();
			}
			else
			{
				return 0;
			}
		}

		public int Update(T item)
		{
			if (_items.Contains(item))
			{
				var itemFromList = _items.ElementAt(_items.IndexOf(item));
				if (!ReferenceEquals(itemFromList, item))
				{
					// The items are "equal" but do not refer to the same instance.
					// Somebody overrode Equals on the type passed as an argument. Replace:
					int index = _items.IndexOf(item);
					_items.RemoveAt(index);
					_items.Insert(index, item);
				}
			}
			else
			{
				var properties = item.GetType().GetProperties();
				var itemKeyProperty = properties.FirstOrDefault(p => p.Name == KeyName);
				if (itemKeyProperty != null)
				{
					var itemKeyValue = itemKeyProperty.GetValue(item, null);
				    var query = from record in _items
				        let c = record.ToDictionary()
				        where c[KeyName].Equals(itemKeyValue)
				        select record;
                    var found = query.FirstOrDefault();
				    if (found != null)
				    {
                        _items.Remove(found);
                    }
                    _items.Add(item);
                }
				else
				{
					throw new Exception("The Key property for the object to be updated is null or is not defined.");
				}
			}
			if (this.FlushToDisk())
			{
				return 1;
			}
			return 0;
		}

		public virtual int Update(IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				if (_items.Contains(item))
				{
					var itemFromList = _items.ElementAt(_items.IndexOf(item));
					if (!ReferenceEquals(itemFromList, item))
					{
						// The items are "equal" but do not refer to the same instance.
						// Somebody overrode Equals on the type passed as an argument. Replace:
						int index = _items.IndexOf(item);
						_items.RemoveAt(index);
						_items.Insert(index, item);
					}
					// Otherwise, the item passed is reference-equal. item now refers to it. Process as normal
				}
				else
				{
					// The item is NOT reference equal to an item in the data store:
					var properties = item.GetType().GetProperties();
					var itemKeyProperty = properties.FirstOrDefault(p => p.Name == KeyName);
					if (itemKeyProperty != null)
					{
						var itemKeyValue = itemKeyProperty.GetValue(item, null);
						foreach (var dataItem in _items)
						{
							// Keep track of those already updated. This might matter over a large
							// dataset:
							var alreadyUpdated = new List<T>();
							if (!alreadyUpdated.Contains(dataItem))
							{
								var dataItemAsDictionary = dataItem.ToDictionary();
								if (dataItemAsDictionary[ KeyName ].Equals(itemKeyValue))
								{
									int index = _items.IndexOf(dataItem);
									_items.Remove(dataItem);
									_items.Insert(index, item);
									alreadyUpdated.Add(dataItem);
									break;
								}
							}
						}
					}
					else
					{
						throw new Exception("The Key property for the object to be updated is null or is not defined.");
					}
				}
			}
			if (this.FlushToDisk())
			{
				return 1;
			}
			return 0;
		}

		public virtual int Delete(T item)
		{
			if (_items.Contains(item))
			{
				var itemFromList = _items.ElementAt(_items.IndexOf(item));
				_items.Remove(item);
			}
			else
			{
				var properties = item.GetType().GetProperties();
				var itemKeyProperty = properties.FirstOrDefault(p => p.Name == KeyName);
				if (itemKeyProperty != null)
				{
					var itemKeyValue = itemKeyProperty.GetValue(item, null);
					foreach (var dataItem in _items)
					{
						var dataItemAsDictionary = dataItem.ToDictionary();
						if (dataItemAsDictionary[ KeyName ].Equals(itemKeyValue))
						{
							int index = _items.IndexOf(dataItem);
							_items.Remove(dataItem);
							break;
						}
					}
				}
				else
				{
					throw new Exception("The Key property for the object to be updated is null or is not defined.");
				}
			}
			if (this.FlushToDisk())
			{
				return 1;
			}
			return 0;
		}

		public virtual int Delete(IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				if (_items.Contains(item))
				{
					_items.Remove(item);
				}
				else
				{
					// The item is NOT reference equal to an item in the data store:
					var properties = item.GetType().GetProperties();
					var itemKeyProperty = properties.FirstOrDefault(p => p.Name == KeyName);
					if (itemKeyProperty != null)
					{
						var itemKeyValue = itemKeyProperty.GetValue(item, null);
						foreach (var dataItem in _items)
						{
							// Keep track of those already updated. This might matter over a large
							// dataset:
							var alreadyUpdated = new List<T>();
							if (!alreadyUpdated.Contains(dataItem))
							{
								var dataItemAsDictionary = dataItem.ToDictionary();
								if (dataItemAsDictionary[ KeyName ].Equals(itemKeyValue))
								{
									int index = _items.IndexOf(dataItem);
									_items.Remove(dataItem);
									alreadyUpdated.Add(dataItem);
									break;
								}
							}
						}
					}
					else
					{
						throw new Exception("The Key property for the object to be updated is null or is not defined.");
					}
				}
			}
			if (this.FlushToDisk())
			{
				return items.Count();
			}
			return 0;
		}

		public virtual int DeleteAll()
		{
			int qtyItems = _items.Count();
			_items.Clear();
			if (this.FlushToDisk())
			{
				return qtyItems;
			}
			return 0;
		}

		public virtual void SetKeyValue(T item, object value)
		{
			var props = item.GetType().GetProperties();
			if (item is ExpandoObject)
			{
				var d = item as IDictionary<string, object>;
				d[ this.KeyName ] = value;
			}
			else
			{
				var pkProp = this.KeyProperty;
				var converted = Convert.ChangeType(value, pkProp.PropertyType);
				pkProp.SetValue(item, converted, null);
			}
		}

		public List<T> TryLoadData()
		{
			List<T> result = new List<T>();
			if (File.Exists(this.DbPath))
			{
				//format for the deserializer...
				var json = File.ReadAllText(this.DbPath);
				result = JsonConvert.DeserializeObject<List<T>>(json);
			    if (result == null)
			    {
			        result = new List<T>();
                }

            }
			_items = result.ToList();
			return result;
		}

		protected virtual object GetKeyValue(T item)
		{
			var property = this.KeyProperty;
			return property.GetValue(item, null);
		}

		protected virtual bool DecideKeyIsAutoIncrementing()
		{
			var info = this.GetKeyProperty();
			var propertyType = info.PropertyType;

			// Key needs to be int, string:
			if (propertyType != typeof(int)
			  && propertyType != typeof(string))
			{
				throw new Exception("key must be either int or string");
			}
			// Decoration with an attribute overrides everything else:
			var attributes = info.GetCustomAttributes(false);
			if (attributes != null && attributes.Count() > 0)
			{
				var attribute = info.GetCustomAttributes(false).First(a => a.GetType() == typeof(PrimaryKeyAttribute));
				var pkAttribute = attribute as PrimaryKeyAttribute;
				if (pkAttribute.IsAutoIncrementing && propertyType == typeof(string))
				{
					throw new Exception("A string key cannot be auto-incrementing. Set the 'IsAuto' Property on the PrimaryKey Attribute to False");
				}
				return pkAttribute.IsAutoIncrementing;
			}
			// Default for int is auto:
			if (propertyType == typeof(int))
			{
				return true;
			}
			// Default for any other type is false, unless overridden with attribute:
			return false;
		}

		protected virtual string GetKeyName()
		{
			var info = this.GetKeyProperty();
			return info.Name;
		}

		protected virtual Type GetKeyType()
		{
			var info = this.GetKeyProperty();
			return info.PropertyType;
		}

		protected virtual PropertyInfo GetKeyProperty()
		{
			var myObject = new T();
			var myType = myObject.GetType();
			var myProperties = myType.GetProperties();
			string objectTypeName = myType.Name;
			PropertyInfo pkProperty = null;

			// Decoration with a [PrimaryKey] attribute overrides everything else:
			var foundProps = myProperties.Where(p => p.GetCustomAttributes(false)
				  .Any(a => a.GetType() == typeof(PrimaryKeyAttribute)));

			if (foundProps != null && foundProps.Count() > 0)
			{
				// For now, more than one pk attribute is a problem:
				if (foundProps.Count() > 1)
				{
					var names = (from p in foundProps select p.Name).ToArray();
					string namelist = "";
					foreach (var pk in foundProps)
					{
						namelist = string.Join(",", names);
					}
					string keyIsAmbiguousMessageFormat = ""
							 + "The key property for {0} is ambiguous between {1}. Please define a single key property.";
					throw new Exception(string.Format(keyIsAmbiguousMessageFormat, objectTypeName, namelist));
				}
				else
				{
					pkProperty = foundProps.ElementAt(0);
				}
			}
			else
			{
				// Is there a property named id (case irrelevant)?
				pkProperty = myProperties
				  .FirstOrDefault(n => n.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase));
				if (pkProperty == null)
				{
					// Is there a property named TypeNameId (case irrelevant)?
					string findName = string.Format("{0}{1}", objectTypeName, "id");
					pkProperty = myProperties
					  .FirstOrDefault(n => n.Name.Equals(findName, StringComparison.CurrentCultureIgnoreCase));
				}
				if (pkProperty == null)
				{
					string keyNotDefinedMessageFormat = ""
							 + "No key property is defined on {0}. Please define a property which forms a unique key for objects of this type.";
					throw new Exception(string.Format(keyNotDefinedMessageFormat, objectTypeName));
				}
			}
			return pkProperty;
		}

		public bool FlushToDisk()
		{
			var completed = false;
			IsFlushing = true;
            // Serialize json directly to the output stream
           
            var tries = 20;
			for (int numTries = 0; numTries <= tries; numTries++)
			{
				try
				{
					var stream = new FileStream(DbPath, FileMode.Create, FileAccess.ReadWrite);

					using (var outstream = new StreamWriter(stream))
					{
						var writer     = new JsonTextWriter(outstream);
                        var serializer = JsonSerializerSettings == null ?
                            JsonSerializer.CreateDefault() :
                            JsonSerializer.CreateDefault(JsonSerializerSettings);
                        serializer.Serialize(writer, _items);
						// outstream.Close();
						completed = true;
					}
				}
				catch (IOException)
				{
					if (numTries == tries)
					{
						throw;
					}
					else
					{
						Thread.Sleep(100);
					}
				}
			}
			IsFlushing = false;
			return completed;
		}
	}
}