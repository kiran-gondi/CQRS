using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Utils
{
  public sealed class Config
  {
    public int NumberOfDatabaseRetries { get; }

    public Config(int numberOfDatabaseRetries)
    {
      NumberOfDatabaseRetries = numberOfDatabaseRetries;
    }
  }
}
