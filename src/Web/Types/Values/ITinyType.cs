using System;
using System.Collections.Generic;

namespace Web.Types.Values
{
    public interface ITinyType<out T>
    {
        T Value { get; }
    }
}
