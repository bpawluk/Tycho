using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class TypeParameterExtractor
    {
        public static TypeParameterModel Extract(ITypeParameterSymbol typeParameterSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            return new TypeParameterModel(
                typeParameterSymbol.Name,
                TypeParameterConstraintsExtractor.Extract(typeParameterSymbol, context));
        }
    }
}
