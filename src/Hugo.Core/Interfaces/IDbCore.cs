using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Hugo.Core {
  public interface IDbCore {
    string ConnectionString { get; set; }
    List<DbColumnMapping> DbColumnsList { get; set; }
    string DbDelimiterFormatString { get; }
    List<string> DbTableNames { get; set; }

    IDbCommand BuildCommand(string sql, params object[] args);
    IEnumerable<T> Execute<T>(string sql, params object[] args) where T : new();
    IEnumerable<dynamic> ExecuteDynamic(string sql, params object[] args);
    object ExecuteScalar(string sql, params object[] args);
    T ExecuteSingle<T>(string sql, params object[] args) where T : new();
    dynamic ExecuteSingleDynamic(string sql, params object[] args);
    IDataReader OpenReader(string sql, params object[] args);

    DBTableMapping getTableMappingFor<T>() where T : new();
    void LoadSchemaInfo();

    int Transact(params System.Data.IDbCommand[] cmds);
    int Transact(string sql, params object[] args);
    IDbConnection CreateConnection(string connectionStringName);
    IDbCommand CreateCommand();

    bool TableExists(string tableName);
    int TryDropTable(string tableName);
    int TransactDDL(string sql, params object[] args);
    int TransactDDL(params System.Data.IDbCommand[] cmds);

    IDataStore<T> CreateRelationalStoreFor<T>() where T : new();
    IDataStore<T> CreateDocumentStoreFor<T>() where T : new();
  }
}