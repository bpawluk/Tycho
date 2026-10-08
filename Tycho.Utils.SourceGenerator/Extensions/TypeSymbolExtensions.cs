using Microsoft.CodeAnalysis;

namespace Tycho.Utils.SourceGenerator.Extensions
{
    internal static class TypeSymbolExtensions
    {
        public static bool InheritsFrom(this ITypeSymbol type, ITypeSymbol baseType)
        {
            for (ITypeSymbol current = type; current != null; current = current.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(current, baseType))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
