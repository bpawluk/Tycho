using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class TypeModelExtractor
    {
        public static TypeModel Extract(ITypeSymbol typeSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            return new TypeModel(
                GetNamespace(typeSymbol),
                GetContainingTypes(typeSymbol, context),
                TypeKindExtractor.Extract(typeSymbol, context),
                TypeModifiersExtractor.Extract(typeSymbol, context),
                typeSymbol.Name,
                TypeParametersExtractor.Extract(typeSymbol, context),
                TypeArgumentsModelExtractor.Extract(typeSymbol, context));
        }

        private static string GetNamespace(ITypeSymbol typeSymbol)
        {
            return typeSymbol.ContainingNamespace
                .ToDisplayString(SymbolDisplayFormat
                .FullyQualifiedFormat
                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));
        }

        private static ImmutableEquatableArray<TypeModel> GetContainingTypes(ITypeSymbol typeSymbol, ExtractorContext context)
        {
            var containingTypes = new Stack<TypeModel>();
            for (INamedTypeSymbol containingTypeSymbol = typeSymbol.ContainingType;
                containingTypeSymbol != null;
                containingTypeSymbol = containingTypeSymbol.ContainingType)
            {
                containingTypes.Push(new TypeModel(
                    GetNamespace(containingTypeSymbol),
                    ImmutableEquatableArray<TypeModel>.Empty,
                    TypeKindExtractor.Extract(containingTypeSymbol, context),
                    TypeModifiersExtractor.Extract(containingTypeSymbol, context),
                    containingTypeSymbol.Name,
                    TypeParametersExtractor.Extract(containingTypeSymbol, context),
                    TypeArgumentsModelExtractor.Extract(containingTypeSymbol, context)));
            }
            return containingTypes.ToImmutableEquatableArray();
        }       
    }
}
