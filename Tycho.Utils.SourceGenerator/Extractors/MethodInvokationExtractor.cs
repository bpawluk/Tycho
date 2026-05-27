using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Models.System;

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
                TypeArgumentsModelExtractor.Extract(methodSymbol, context));
        }

        private static TypeModel? GetReceiverType(IMethodSymbol methodSymbol, ExtractorContext context)
        {
            if (methodSymbol.ReceiverType is ITypeSymbol receiverTypeSymbol)
            {
                return TypeModelExtractor.Extract(receiverTypeSymbol, context);
            }
            return null;
        }
    }
}
