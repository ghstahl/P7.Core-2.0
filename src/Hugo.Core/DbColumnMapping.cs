using System;
using System.Linq;

namespace Hugo.Core {
  public class DbColumnMapping {
    private string _delimeterFormatString;

    public DbColumnMapping(string delimiterFormatString) {
      _delimeterFormatString = delimiterFormatString;
      this.IsAutoIncementing = false;
      this.IsPrimaryKey = false;
    }

    public bool IsAutoIncementing { get; set; }
    public bool IsPrimaryKey { get; set; }
    public Type DataType { get; set; }
    public string TableName { get; set; }
    public string ColumnName { get; set; }
    public string PropertyName { get; set; }

    public string DelimitedColumnName {
      get { return string.Format(_delimeterFormatString, this.ColumnName); }
    }
  }
}