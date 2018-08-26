using System;
using System.Collections.Generic;
using System.Text;

namespace WeText.Common.Events.Domain
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class HandlesInlineAttribute : Attribute
    {
    }
}
