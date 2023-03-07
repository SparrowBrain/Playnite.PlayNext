using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.GameActivity.Helpers
{
    public static class EnumerableExtensions
    {
        public static ulong Sum<T>(this IEnumerable<T> items, Func<T, ulong> func)
        {
            return items.Aggregate<T, ulong>(0, (current, item) => current + func.Invoke(item));
        }
    }
}