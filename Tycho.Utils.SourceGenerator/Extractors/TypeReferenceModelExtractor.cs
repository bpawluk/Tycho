using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class TypeReferenceModelExtractor
    {
        public static TypeReferenceModel Extract(ITypeSymbol typeSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (typeSymbol is ITypeParameterSymbol)
            {
                return TypeReferenceModel.TypeParameter(GetNamespace(typeSymbol), typeSymbol.Name);
            }

            if (typeSymbol is INamedTypeSymbol)
            {
                return new TypeReferenceModel(
                    GetNamespace(typeSymbol),
                    GetContainingTypes(typeSymbol, context),
                    typeSymbol.Name,
                    TypeArgumentsModelExtractor.Extract(typeSymbol, context));
            }

            return new TypeReferenceModel(
                GetNamespace(typeSymbol),
                ImmutableEquatableArray<TypeReferenceModel>.Empty,
                typeSymbol.Name,
                TypeArgumentsModelExtractor.Extract(typeSymbol, context));
        }

        private static string GetNamespace(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.ContainingNamespace == null)
            {
                return string.Empty;
            }

            return typeSymbol.ContainingNamespace.ToDisplayString(
                SymbolDisplayFormat
                    .FullyQualifiedFormat
                    .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));
        }

        private static ImmutableEquatableArray<TypeReferenceModel> GetContainingTypes(ITypeSymbol typeSymbol, ExtractorContext context)
        {
            var containingTypes = new Stack<TypeReferenceModel>();
            for (INamedTypeSymbol containingTypeSymbol = typeSymbol.ContainingType;
                containingTypeSymbol != null;
                containingTypeSymbol = containingTypeSymbol.ContainingType)
            {
                containingTypes.Push(new TypeReferenceModel(
                    GetNamespace(containingTypeSymbol),
                    ImmutableEquatableArray<TypeReferenceModel>.Empty,
                    containingTypeSymbol.Name,
                    TypeArgumentsModelExtractor.Extract(containingTypeSymbol, context)));
            }
            return containingTypes.ToImmutableEquatableArray();
        }
    }
}
