using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class TypeDefinitionModelExtractor
    {
        public static TypeDefinitionModel Extract(ITypeSymbol typeSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            return new TypeDefinitionModel(
                GetNamespace(typeSymbol),
                GetContainingTypes(typeSymbol, context),
                TypeKindExtractor.Extract(typeSymbol, context),
                TypeModifiersExtractor.Extract(typeSymbol, context),
                typeSymbol.Name,
                TypeParametersExtractor.Extract(typeSymbol, context));
        }

        private static string GetNamespace(ITypeSymbol typeSymbol)
        {
            return typeSymbol.ContainingNamespace
                .ToDisplayString(SymbolDisplayFormat
                .FullyQualifiedFormat
                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));
        }

        private static ImmutableEquatableArray<TypeDefinitionModel> GetContainingTypes(ITypeSymbol typeSymbol, ExtractorContext context)
        {
            var containingTypes = new Stack<TypeDefinitionModel>();
            for (INamedTypeSymbol containingTypeSymbol = typeSymbol.ContainingType;
                containingTypeSymbol != null;
                containingTypeSymbol = containingTypeSymbol.ContainingType)
            {
                containingTypes.Push(new TypeDefinitionModel(
                    GetNamespace(containingTypeSymbol),
                    ImmutableEquatableArray<TypeDefinitionModel>.Empty,
                    TypeKindExtractor.Extract(containingTypeSymbol, context),
                    TypeModifiersExtractor.Extract(containingTypeSymbol, context),
                    containingTypeSymbol.Name,
                    TypeParametersExtractor.Extract(containingTypeSymbol, context)));
            }
            return containingTypes.ToImmutableEquatableArray();
        }
    }
}
