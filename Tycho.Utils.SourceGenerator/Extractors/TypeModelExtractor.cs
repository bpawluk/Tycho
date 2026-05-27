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

            Models.System.TypeKind typeKind = TypeKindExtractor.Extract(typeSymbol, context);
            return new TypeModel(
                GetNamespace(typeSymbol),
                GetContainingTypes(typeSymbol, context),
                typeKind,
                TypeModifiersExtractor.Extract(typeSymbol, typeKind, context),
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
                Models.System.TypeKind typeKind = TypeKindExtractor.Extract(containingTypeSymbol, context);
                containingTypes.Push(new TypeModel(
                    GetNamespace(containingTypeSymbol),
                    ImmutableEquatableArray<TypeModel>.Empty,
                    typeKind,
                    TypeModifiersExtractor.Extract(containingTypeSymbol, typeKind, context),
                    containingTypeSymbol.Name,
                    TypeParametersExtractor.Extract(containingTypeSymbol, context),
                    TypeArgumentsModelExtractor.Extract(containingTypeSymbol, context)));
            }
            return containingTypes.ToImmutableEquatableArray();
        }       
    }
}
