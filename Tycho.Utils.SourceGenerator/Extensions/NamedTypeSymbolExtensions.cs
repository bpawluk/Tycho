using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extensions
{
    internal static class NamedTypeSymbolExtensions
    {
        public static ImmutableEquatableArray<string> GetTypeParameters(this INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeParameters.Length == 0)
            {
                return ImmutableEquatableArray<string>.Empty;
            }
            return typeSymbol.TypeParameters.Select(parameter => parameter.Name).ToImmutableEquatableArray();
        }

        public static ImmutableEquatableArray<string> GetTypeArguments(this INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeArguments.Length == 0)
            {
                return ImmutableEquatableArray<string>.Empty;
            }
            return typeSymbol.TypeArguments.Select(GetTypeArgumentReferenceName).ToImmutableEquatableArray();
        }

        public static ImmutableEquatableArray<string> GetTypeParameterConstraintClauses(this INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeParameters.Length == 0)
            {
                return ImmutableEquatableArray<string>.Empty;
            }
            return typeSymbol.TypeParameters.Select(GetTypeParameterConstraintClause).Where(clause => !string.IsNullOrWhiteSpace(clause)).ToImmutableEquatableArray();
        }

        public static Models.System.TypeKind GetContainingTypeKind(this INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.IsRecord)
            {
                return typeSymbol.IsValueType
                    ? Models.System.TypeKind.RecordStruct
                    : Models.System.TypeKind.RecordClass;
            }

            return typeSymbol.TypeKind switch
            {
                TypeKind.Interface => Models.System.TypeKind.Interface,
                TypeKind.Struct => Models.System.TypeKind.Struct,
                _ => Models.System.TypeKind.Class,
            };
        }

        public static ImmutableEquatableArray<string> GetContainingTypeModifiers(this INamedTypeSymbol typeSymbol, Models.System.TypeKind kind)
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
