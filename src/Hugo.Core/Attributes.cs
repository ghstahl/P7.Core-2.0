using System;
using System.Linq;

namespace Hugo.Core {
  public class PrimaryKeyAttribute : Attribute {
    public PrimaryKeyAttribute(bool Auto) {
      this.IsAutoIncrementing = Auto;
    }
    public bool IsAutoIncrementing { get; private set; }
  }

  public class DbColumnAttribute : Attribute {
    public DbColumnAttribute(string name) {
      this.Name = name;
    }
    public string Name { get; protected set; }
  }

  public class DbTableAttribute : Attribute {
    public DbTableAttribute(string name) {
      this.Name = name;
    }
    public string Name { get; protected set; }
  }
}