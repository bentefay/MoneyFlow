﻿using System.Collections.Generic;

namespace Build
{
    public static class StringExtensions 
    {
        public static string Join(this IEnumerable<object> source, string separator)
        {
            return string.Join(separator, source);
        }
    }
}