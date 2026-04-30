using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library
{
    public static class CxObjectHelper
    {
        public static object? GetDefault(this Type t)
        {
            var defaultValue = typeof(CxObjectHelper)
                .GetMethod(nameof(GetDefault), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(t).Invoke(null, null);
            return defaultValue;
        }

        private static T? GetDefault<T>()
        {
            return default;
        }
    }
}
