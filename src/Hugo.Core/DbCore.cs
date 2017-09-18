using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Hugo.Extensions;
using Hugo.Utils;
using Microsoft.Extensions.Configuration;

namespace Hugo.Core
{
	public abstract class DbCore : IDbCore
	{
		public virtual string ConnectionString { get; set; }

		public virtual List<DbColumnMapping> DbColumnsList { get; set; }
		public virtual List<string> DbTableNames { get; set; }
		public abstract string DbDelimiterFormatString { get; }

		public abstract bool TableExists(string tableName);
		protected abstract void LoadDbColumnsList();
		protected abstract void LoadDbTableNames();
		public abstract IDbConnection CreateConnection(string connectionStringName);
		public abstract IDbCommand CreateCommand();

		public static IConfiguration Config { get; }

		static DbCore()
		{
			Config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables()
				.Build();
		}

		protected DbCore() { }

		public DbCore(string connectionStringName)
		{
			try
			{
				//this.ConnectionString = ConfigurationManager.ConnectionStrings[ connectionStringName ].ConnectionString;
				this.ConnectionString = Config.GetConnectionString(connectionStringName);
			}
			catch
			{
				throw new InvalidOperationException("Can't find the connection string " + connectionStringName);
			}
			this.LoadSchemaInfo();
		}

		public virtual void LoadSchemaInfo()
		{
			this.LoadDbTableNames();
			this.LoadDbColumnsList();
		}

		public virtual DBTableMapping getTableMappingFor<T>() where T : new()
		{
			var result = new DBTableMapping(this.DbDelimiterFormatString);
			var item = new T();
			var itemType = item.GetType();
			var properties = itemType.GetProperties();
			string replaceString = "[^a-zA-Z0-9]";
			var rgx = new Regex(replaceString);

			// Set up the default, trying to simple map type name to table name:
			string flattenedItemTypeName = rgx.Replace(itemType.Name.ToLower(), "");
			string plural = Inflector.Pluralize(flattenedItemTypeName);
			var dbTableName = this.DbTableNames.FirstOrDefault(t => rgx.Replace(t.ToLower(), "") == flattenedItemTypeName);
			if (dbTableName == null)
			{
				dbTableName = this.DbTableNames.FirstOrDefault(t => rgx.Replace(t.ToLower(), "") == plural);
			}
			// Override the default if the user specified a name with an attribute:
			var tableNameAttribute = itemType.GetTypeInfo().GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(DbTableAttribute)) as DbTableAttribute;
			if (tableNameAttribute != null)
			{
				dbTableName = tableNameAttribute.Name;
			}
			// If it's still null, there is no mapping found:
			if (dbTableName == null)
			{
				throw new Exception(string.Format("Could not map class '{0}' to a table in the database.", itemType.Name));
			}
			result.DBTableName = dbTableName;
			result.MappedTypeName = itemType.Name;
			var dbColumnInfo = from c in this.DbColumnsList where c.TableName == dbTableName select c;
			foreach (var property in properties)
			{
				var propertyType = property.PropertyType;
				string flattenedPropertyName = rgx.Replace(property.Name.ToLower(), "");
				DbColumnMapping columnMapping = dbColumnInfo.FirstOrDefault(c => rgx.Replace(c.ColumnName.ToLower(), "") == flattenedPropertyName);
				if (columnMapping != null)
				{
					columnMapping.PropertyName = property.Name;
					columnMapping.DataType = propertyType;
				}
				// Look for a custom column name attribute:
				DbColumnAttribute mappedColumnAttribute = null;
				var attribute = property.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(DbColumnAttribute));
				if (attribute != null)
				{
					// Use the column name found in the attribute:
					mappedColumnAttribute = attribute as DbColumnAttribute;
					string matchColumnName = mappedColumnAttribute.Name;
					columnMapping = dbColumnInfo.FirstOrDefault(c => c.ColumnName == matchColumnName);
					columnMapping.PropertyName = property.Name;
					columnMapping.DataType = propertyType;
				}
				if (columnMapping != null)
				{
					result.ColumnMappings.Add(columnMapping);
					if (columnMapping.IsPrimaryKey)
					{
						result.PrimaryKeyMapping.Add(columnMapping);
					}
				}
			}
			if (result.PrimaryKeyMapping.Count == 0)
			{
				string keyNotDefinedMessageFormat = ""
		  + "No primary key mapping found in table '{0}' for type '{1}'. "
		  + "Please define a property which maps to a table primary key for objects of this type.";
				throw new Exception(string.Format(keyNotDefinedMessageFormat, dbTableName, itemType.Name));
			}
			return result;
		}

		/// <summary>
		/// Returns a single record, typed as you need
		/// </summary>
		public virtual T ExecuteSingle<T>(string sql, params object[] args) where T : new()
		{
			return this.Execute<T>(sql, args).FirstOrDefault();
		}

		/// <summary>
		/// Returns a simple ExpandoObject with all results of a query
		/// </summary>
		public virtual dynamic ExecuteSingleDynamic(string sql, params object[] args)
		{
			return this.ExecuteDynamic(sql, args).First();
		}

		/// <summary>
		/// Executes a typed query
		/// </summary>
		public virtual IDataReader OpenReader(string sql, params object[] args)
		{
			//var conn = new NpgsqlConnection(this.ConnectionString);
			var conn = this.CreateConnection(this.ConnectionString);
			var cmd = BuildCommand(sql, args);
			cmd.Connection = conn;
			//defer opening to the last minute
			conn.Open();
			//use a rdr here and yield back the projection
			//connection will close when rdr is finished
			var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
			return rdr;
		}

		/// <summary>
		/// Executes a typed query
		/// </summary>
		public virtual IEnumerable<T> Execute<T>(string sql, params object[] args) where T : new()
		{
			using (var rdr = OpenReader(sql, args))
			{
				while (rdr.Read())
				{
					yield return rdr.ToSingle<T>();
				}
				rdr.Dispose();
			}
		}

		/// <summary>
		/// Executes a query returning items in a dynamic list
		/// </summary>
		public virtual IEnumerable<dynamic> ExecuteDynamic(string sql, params object[] args)
		{
			using (var rdr = OpenReader(sql, args))
			{
				while (rdr.Read())
				{
					yield return rdr.RecordToExpando(); ;
				}
				rdr.Dispose();
			}
		}

		/// <summary>
		/// Convenience method for building a command
		/// </summary>
		/// <param name="sql">The SQL to execute with param names as @0, @1, @2 etc</param>
		/// <param name="args">The parameters to match the @ notations</param>
		/// <returns></returns>
		public virtual IDbCommand BuildCommand(string sql, params object[] args)
		{
			//var cmd = new NpgsqlCommand(sql);
			var cmd = this.CreateCommand();
			cmd.CommandText = sql;
			cmd.AddParams(args);
			return cmd;
		}

		/// <summary>
		/// A Transaction helper that executes a series of commands in a single transaction
		/// </summary>
		/// <param name="cmds">Commands built with BuildCommand</param>
		/// <returns></returns>
		public virtual int Transact(params IDbCommand[] cmds)
		{
			var results = new List<int>();
			using (var conn = this.CreateConnection(this.ConnectionString))
			{
				conn.Open();
				using (var tx = conn.BeginTransaction())
				{
					try
					{
						foreach (var cmd in cmds)
						{
							cmd.Transaction = tx;
							cmd.Connection = conn;
							results.Add(cmd.ExecuteNonQuery());
							cmd.Dispose();
						}
						tx.Commit();
					}
					catch (DbException x)
					{
						tx.Rollback();
						throw (x);
					}
					finally
					{
						conn.Close();
					}
				}
			}
			return results.Sum();
		}

		public virtual int Transact(string sql, params object[] args)
		{
			var cmd = this.BuildCommand(sql, args);
			return Transact(cmd);
		}

		public virtual object ExecuteScalar(string sql, params object[] args)
		{
			using (var conn = this.CreateConnection(this.ConnectionString))
			{
				using (var cmd = this.BuildCommand(sql, args))
				{
					conn.Open();
					cmd.Connection = conn;
					try
					{
						return cmd.ExecuteScalar();
					}
					catch (DbException x)
					{
						throw (x);
					}
					finally
					{
						conn.Close();
					}
				}
			}
		}

		public virtual int TryDropTable(string tableName)
		{
			if (TableExists(tableName))
			{
				string sql = string.Format("DROP TABLE \"{0}\"", tableName);
				var cmd = this.BuildCommand(sql);
				try
				{
					return this.TransactDDL(cmd);
				}
				catch (DbException x)
				{
					if (x.Message.Contains("does not exist"))
					{
						return 0;
					}
					else throw (x);
				}
			}
			return 1;
		}

		public int TransactDDL(string sql, params object[] args)
		{
			var result = this.Transact(sql, args);
			this.LoadSchemaInfo();
			return result;
		}

		public int TransactDDL(params IDbCommand[] cmds)
		{
			var result = this.Transact(cmds);
			this.LoadSchemaInfo();
			return result;
		}

		public abstract IDataStore<T> CreateRelationalStoreFor<T>() where T : new();
		public abstract IDataStore<T> CreateDocumentStoreFor<T>() where T : new();
	}
}