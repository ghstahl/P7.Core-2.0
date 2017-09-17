using System;
using System.Collections.Generic;
using System.Linq;

namespace Hugo.Core {
  public class DbColumnMappingLookup {
    private Dictionary<string, DbColumnMapping> ByProperty;
    private Dictionary<string, DbColumnMapping> ByColumn;
    private string _delimiterFormatString;

    public Dictionary<string, DbColumnMapping> ColumnsByPropertyName { get { return ByProperty; } }
    public Dictionary<string, DbColumnMapping> ColumnsByColumnName { get { return ByColumn; } }

    public DbColumnMappingLookup(string NameDelimiterFormatString) {
      _delimiterFormatString = NameDelimiterFormatString;
      this.ByProperty = new Dictionary<string, DbColumnMapping>(StringComparer.CurrentCultureIgnoreCase);
      this.ByColumn = new Dictionary<string, DbColumnMapping>(StringComparer.CurrentCultureIgnoreCase);
    }

    public int Count() {
      return this.ByProperty.Count();
    }

    public DbColumnMapping Add(string columnName, string propertyName) {
      string delimited = string.Format(_delimiterFormatString, columnName);
      var mapping = new DbColumnMapping(_delimiterFormatString);
      mapping.ColumnName = columnName;
      mapping.PropertyName = propertyName;

      // add the same instance to both dictionaries:
      this.ByColumn.Add(mapping.ColumnName, mapping);
      this.ByProperty.Add(mapping.PropertyName, mapping);
      return mapping;
    }

    public DbColumnMapping Add(DbColumnMapping mapping) {
      this.ByColumn.Add(mapping.ColumnName, mapping);
      this.ByProperty.Add(mapping.PropertyName, mapping);
      return mapping;
    }

    public DbColumnMapping FindByColumn(string columnName) {
      DbColumnMapping mapping;
      this.ByColumn.TryGetValue(columnName, out mapping);
      return mapping;
    }

    public DbColumnMapping FindByProperty(string propertyName) {
      DbColumnMapping mapping;
      this.ByProperty.TryGetValue(propertyName, out mapping);
      return mapping;
    }

    public bool ContainsPropertyName(string propertyName) {
      return this.ByProperty.ContainsKey(propertyName);
    }

    public bool ContainsColumnName(string columnName) {
      return this.ByColumn.ContainsKey(columnName);
    }
  }
}