using System.Linq;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class TypeParametersExtractor
    {
        public static ImmutableEquatableArray<TypeParameterModel> Extract(ITypeSymbol typeSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeParameters.Length > 0)
            {
                return namedTypeSymbol.TypeParameters
                    .Select(typeParameter => TypeParameterExtractor.Extract(typeParameter, context))
                    .ToImmutableEquatableArray();
            }

            return ImmutableEquatableArray<TypeParameterModel>.Empty;
        }
    }
}
