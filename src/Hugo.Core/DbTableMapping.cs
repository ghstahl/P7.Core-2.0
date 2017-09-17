using System;
using System.Collections.Generic;
using System.Linq;

namespace Hugo.Core {
  public class DBTableMapping {
    private string _delimiterFormatString;

    public List<DbColumnMapping> PrimaryKeyMapping { get; set; }
    public DbColumnMappingLookup ColumnMappings { get; set; }
    public string DBTableName { get; set; }
    public string MappedTypeName { get; set; }

    public DBTableMapping(string delimiterFormatString) {
      _delimiterFormatString = delimiterFormatString;
      this.ColumnMappings = new DbColumnMappingLookup(_delimiterFormatString);
      this.PrimaryKeyMapping = new List<DbColumnMapping>();
    }

    public string DelimitedTableName {
      get { return string.Format(_delimiterFormatString, this.DBTableName); }
    }

    public bool HasCompoundPk {
      get {
        if (this.PrimaryKeyMapping.Count > 1) {
          return true;
        } else {
          return false;
        }
      }
    }
  }
}