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

        private static ImmutableEquatableArray<TypeReferenceModel> GetParameterTypes(IMethodSymbol methodSymbol, ExtractorContext context)
        {
            return methodSymbol.Parameters
                .Select(paramSymbol => TypeReferenceModelExtractor.Extract(paramSymbol.Type, context))
                .ToImmutableEquatableArray();
        }

        private static TypeReferenceModel GetResultType(IMethodSymbol methodSymbol, ExtractorContext context)
        {
            return TypeReferenceModelExtractor.Extract(methodSymbol.ReturnType, context);
        }
    }
}
