using System;
using System.Collections.Generic;
using System.Linq;

namespace Hugo.Core {
  public class BiggyEventArgs<T> : EventArgs {
    public List<T> Items { get; set; }
    public dynamic Item { get; set; }

    public BiggyEventArgs() {
      Items = new List<T>();
      this.Item = default(T);
    }
  }
}