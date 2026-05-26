using System.Collections.Generic;
using System.Linq;
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
                GetContainingTypes(typeSymbol),
                typeSymbol.Name,
                GetTypeParameters(typeSymbol),
                GetTypeParameterConstraintClauses(typeSymbol),
                GetTypeArguments(typeSymbol));
        }

        private static string GetNamespace(ITypeSymbol typeSymbol)
        {
            return typeSymbol.ContainingNamespace
                .ToDisplayString(SymbolDisplayFormat
                .FullyQualifiedFormat
                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));
        }

        private static ImmutableEquatableArray<ContainingTypeModel> GetContainingTypes(ITypeSymbol typeSymbol)
        {
            var containingTypes = new Stack<ContainingTypeModel>();
            for (INamedTypeSymbol containingTypeSymbol = typeSymbol.ContainingType;
                containingTypeSymbol != null;
                containingTypeSymbol = containingTypeSymbol.ContainingType)
            {
                Models.System.TypeKind kind = GetContainingTypeKind(containingTypeSymbol);
                containingTypes.Push(new ContainingTypeModel(
                    kind,
                    GetContainingTypeModifiers(containingTypeSymbol, kind),
                    containingTypeSymbol.Name,
                    GetTypeParameters(containingTypeSymbol),
                    GetTypeParameterConstraintClauses(containingTypeSymbol),
                    GetTypeArguments(containingTypeSymbol)));
            }
            return containingTypes.ToImmutableEquatableArray();
        }

        private static ImmutableEquatableArray<string> GetTypeParameters(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeParameters.Length > 0)
            {
                return namedTypeSymbol.TypeParameters
                    .Select(parameter => parameter.Name)
                    .ToImmutableEquatableArray();
            }
            return ImmutableEquatableArray<string>.Empty;
        }

        private static ImmutableEquatableArray<string> GetTypeArguments(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeArguments.Length > 0)
            {
                return namedTypeSymbol.TypeArguments
                    .Select(GetTypeArgumentReferenceName)
                    .ToImmutableEquatableArray();
            }
            return ImmutableEquatableArray<string>.Empty;
        }

        private static ImmutableEquatableArray<string> GetTypeParameterConstraintClauses(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeParameters.Length > 0)
            {
                return namedTypeSymbol.TypeParameters
                    .Select(GetTypeParameterConstraintClause)
                    .Where(clause => !string.IsNullOrWhiteSpace(clause))
                    .ToImmutableEquatableArray();
            }
            return ImmutableEquatableArray<string>.Empty;
        }

        private static Models.System.TypeKind GetContainingTypeKind(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsRecord)
            {
                return typeSymbol.IsValueType
                    ? Models.System.TypeKind.RecordStruct
                    : Models.System.TypeKind.RecordClass;
            }

            return typeSymbol.TypeKind switch
            {
                Microsoft.CodeAnalysis.TypeKind.Interface => Models.System.TypeKind.Interface,
                Microsoft.CodeAnalysis.TypeKind.Struct => Models.System.TypeKind.Struct,
                _ => Models.System.TypeKind.Class,
            };
        }

        private static ImmutableEquatableArray<string> GetContainingTypeModifiers(ITypeSymbol typeSymbol, Models.System.TypeKind kind)
        {
            var declarationTokens = new List<string>();

            string accessibilityKeyword = GetAccessibilityKeyword(typeSymbol.DeclaredAccessibility);
            if (!string.IsNullOrEmpty(accessibilityKeyword))
            {
                declarationTokens.Add(accessibilityKeyword);
            }

            if (kind == Models.System.TypeKind.Class || kind == Models.System.TypeKind.RecordClass)
            {
                if (typeSymbol.IsStatic)
                {
                    declarationTokens.Add("static");
                }
                else
                {
                    if (typeSymbol.IsAbstract)
                    {
                        declarationTokens.Add("abstract");
                    }

                    if (typeSymbol.IsSealed)
                    {
                        declarationTokens.Add("sealed");
                    }
                }
            }
            else if (kind == Models.System.TypeKind.Struct || kind == Models.System.TypeKind.RecordStruct)
            {
                if (typeSymbol.IsReadOnly)
                {
                    declarationTokens.Add("readonly");
                }

                if (typeSymbol.IsRefLikeType)
                {
                    declarationTokens.Add("ref");
                }
            }

            declarationTokens.Add("partial");
            return declarationTokens.ToImmutableEquatableArray();
        }

        private static string GetTypeArgumentReferenceName(ITypeSymbol typeArgument)
        {
            if (typeArgument is ITypeParameterSymbol typeParameter)
            {
                return typeParameter.Name;
            }
            return typeArgument.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }

        private static string GetTypeParameterConstraintClause(ITypeParameterSymbol typeParameter)
        {
            var constraints = new List<string>();

            if (typeParameter.HasUnmanagedTypeConstraint)
            {
                constraints.Add("unmanaged");
            }
            else if (typeParameter.HasValueTypeConstraint)
            {
                constraints.Add("struct");
            }
            else if (typeParameter.HasReferenceTypeConstraint)
            {
                string referenceTypeConstraint = typeParameter.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated ? "class?" : "class";
                constraints.Add(referenceTypeConstraint);
            }

            if (typeParameter.HasNotNullConstraint && !constraints.Contains("notnull"))
            {
                constraints.Add("notnull");
            }

            foreach (ITypeSymbol constraintType in typeParameter.ConstraintTypes)
            {
                constraints.Add(constraintType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            }

            if (typeParameter.HasConstructorConstraint)
            {
                constraints.Add("new()");
            }

            if (constraints.Count == 0)
            {
                return string.Empty;
            }

            return $"where {typeParameter.Name} : {string.Join(", ", constraints)}";
        }

        private static string GetAccessibilityKeyword(Accessibility accessibility)
        {
            return accessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Private => "private",
                Accessibility.Internal => "internal",
                Accessibility.Protected => "protected",
                Accessibility.ProtectedOrInternal => "protected internal",
                Accessibility.ProtectedAndInternal => "private protected",
                _ => string.Empty,
            };
        }
    }
}
