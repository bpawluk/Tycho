using System;
using System.Runtime.CompilerServices;

namespace Tycho.Utils
{
    internal static class ExceptionExtensions
    {
        public static void ThrowIfNull<T>(
            this T? argument,
            [CallerArgumentExpression("argument")] string? paramName = null) 
            where T : class
        {
            if (argument is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
