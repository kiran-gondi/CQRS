using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Utils
{
  public sealed class ConnectionString
  {
    public string Value { get; }

    public ConnectionString(string value)
    {
      Value = value;
    }
  }
}
