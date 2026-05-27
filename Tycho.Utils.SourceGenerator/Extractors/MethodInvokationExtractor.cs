using System.Linq;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class MethodInvokationExtractor
    {
        public static MethodInvocationModel Extract(IMethodSymbol methodSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            return new MethodInvocationModel(
                MethodSignatureExtractor.Extract(methodSymbol.OriginalDefinition, context),
                GetReceiverType(methodSymbol, context),
                GetTypeArguments(methodSymbol, context));
        }

        private static TypeModel GetReceiverType(IMethodSymbol methodSymbol, ExtractorContext context)
        {
            if (methodSymbol.ReceiverType is ITypeSymbol receiverTypeSymbol)
            {
                return TypeModelExtractor.Extract(receiverTypeSymbol, context);
            }
            return default;
        }

        private static ImmutableEquatableArray<TypeArgumentModel> GetTypeArguments(IMethodSymbol methodSymbol, ExtractorContext context)
        {
            return methodSymbol.TypeParameters
                .Zip(methodSymbol.TypeArguments, (typeParameter, typeArgument) =>
                    new TypeArgumentModel(
                        typeParameter.Name,
                        TypeModelExtractor.Extract(typeArgument, context)))
                .ToImmutableEquatableArray();
        }
    }
}
