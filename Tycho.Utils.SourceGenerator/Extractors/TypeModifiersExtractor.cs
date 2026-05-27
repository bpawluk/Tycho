using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class TypeModifiersExtractor
    {
        public static ImmutableEquatableArray<TypeModifier> Extract(ITypeSymbol typeSymbol, Models.System.TypeKind kind, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var declarationTokens = new List<TypeModifier>();

            TypeModifier? accessibilityModifier = GetAccessibilityModifier(typeSymbol.DeclaredAccessibility);
            if (accessibilityModifier.HasValue)
            {
                declarationTokens.Add(accessibilityModifier.Value);
            }

            if (kind == Models.System.TypeKind.Class || kind == Models.System.TypeKind.RecordClass)
            {
                if (typeSymbol.IsStatic)
                {
                    declarationTokens.Add(TypeModifier.Static);
                }
                else
                {
                    if (typeSymbol.IsAbstract)
                    {
                        declarationTokens.Add(TypeModifier.Abstract);
                    }

                    if (typeSymbol.IsSealed)
                    {
                        declarationTokens.Add(TypeModifier.Sealed);
                    }
                }
            }
            else if (kind == Models.System.TypeKind.Struct || kind == Models.System.TypeKind.RecordStruct)
            {
                if (typeSymbol.IsReadOnly)
                {
                    declarationTokens.Add(TypeModifier.ReadOnly);
                }

                if (typeSymbol.IsRefLikeType)
                {
                    declarationTokens.Add(TypeModifier.Ref);
                }
            }

            declarationTokens.Add(TypeModifier.Partial);
            return declarationTokens.ToImmutableEquatableArray();
        }

        private static TypeModifier? GetAccessibilityModifier(Accessibility accessibility)
        {
            return accessibility switch
            {
                Accessibility.Public => TypeModifier.Public,
                Accessibility.Private => TypeModifier.Private,
                Accessibility.Internal => TypeModifier.Internal,
                Accessibility.Protected => TypeModifier.Protected,
                Accessibility.ProtectedOrInternal => TypeModifier.ProtectedInternal,
                Accessibility.ProtectedAndInternal => TypeModifier.PrivateProtected,
                _ => null,
            };
        }
    }
}
