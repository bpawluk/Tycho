using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal class MethodDefinitionExtractor
    {
        public static MethodDefinitionModel Extract(TypeModel containingType, IMethodSymbol methodSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            return new MethodDefinitionModel(
                containingType,
                MethodSignatureExtractor.Extract(methodSymbol, context),
                MethodInvokationsExtractor.Extract(methodSymbol, context));
        }
    }
}
