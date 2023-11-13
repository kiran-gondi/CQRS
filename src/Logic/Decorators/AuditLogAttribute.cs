using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Decorators
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
  public sealed class AuditLogAttribute : Attribute
  {
    public AuditLogAttribute()
    {

    }
  }
}
