using System.Linq;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class MethodSignatureExtractor
    {
        public static MethodSignatureModel Extract(IMethodSymbol methodSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            return new MethodSignatureModel(
                methodSymbol.Name,
                GetParameterTypes(methodSymbol, context),
                GetResultType(methodSymbol, context));
        }

        private static ImmutableEquatableArray<TypeModel> GetParameterTypes(IMethodSymbol methodSymbol, ExtractorContext context)
        {
            return methodSymbol.Parameters
                .Select(paramSymbol => TypeModelExtractor.Extract(paramSymbol.Type, context))
                .ToImmutableEquatableArray();
        }

        private static TypeModel GetResultType(IMethodSymbol methodSymbol, ExtractorContext context)
        {
            return TypeModelExtractor.Extract(methodSymbol.ReturnType, context);
        }
    }
}
