using Microsoft.CodeAnalysis;

namespace Tycho.Utils.SourceGenerator.Extensions
{
    internal static class MethodSymbolExtensions
    {
        public static IMethodSymbol GetOriginalDefinition(this IMethodSymbol methodSymbol)
        {
            return (methodSymbol.ReducedFrom ?? methodSymbol).OriginalDefinition;
        }
    }
}
