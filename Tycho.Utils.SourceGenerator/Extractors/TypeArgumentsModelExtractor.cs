using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class TypeArgumentsModelExtractor
    {
        public static ImmutableEquatableArray<TypeArgumentModel> Extract(IMethodSymbol methodSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (methodSymbol.TypeArguments.Length > 0)
            {
                return GetTypeArguments(methodSymbol.TypeParameters, methodSymbol.TypeArguments, context);
            }

            return ImmutableEquatableArray<TypeArgumentModel>.Empty;
        }

        public static ImmutableEquatableArray<TypeArgumentModel> Extract(ITypeSymbol typeSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeArguments.Length > 0)
            {
                return GetTypeArguments(namedTypeSymbol.TypeParameters, namedTypeSymbol.TypeArguments, context);
            }

            return ImmutableEquatableArray<TypeArgumentModel>.Empty;
        }

        private static ImmutableEquatableArray<TypeArgumentModel> GetTypeArguments(
            IEnumerable<ITypeParameterSymbol> typeParameters,
            IEnumerable<ITypeSymbol> typeArguments,
            ExtractorContext context)
        {
            return typeParameters
                .Zip(typeArguments, (typeParameter, typeArgument) =>
                    new TypeArgumentModel(
                        typeParameter.Name,
                        TypeReferenceModelExtractor.Extract(typeArgument, context)))
                .ToImmutableEquatableArray();
        }
    }
}
