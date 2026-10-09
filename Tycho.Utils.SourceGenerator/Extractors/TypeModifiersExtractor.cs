using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class TypeModifiersExtractor
    {
        private static readonly Dictionary<SyntaxKind, TypeModifier> s_modifiers = new()
        {
            { SyntaxKind.NewKeyword, TypeModifier.New },
            { SyntaxKind.PublicKeyword, TypeModifier.Public },
            { SyntaxKind.ProtectedKeyword, TypeModifier.Protected },
            { SyntaxKind.InternalKeyword, TypeModifier.Internal },
            { SyntaxKind.PrivateKeyword, TypeModifier.Private },
            { SyntaxKind.FileKeyword, TypeModifier.File },
            { SyntaxKind.StaticKeyword, TypeModifier.Static },
            { SyntaxKind.VirtualKeyword, TypeModifier.Virtual },
            { SyntaxKind.SealedKeyword, TypeModifier.Sealed },
            { SyntaxKind.OverrideKeyword, TypeModifier.Override },
            { SyntaxKind.AbstractKeyword, TypeModifier.Abstract },
            { SyntaxKind.ExternKeyword, TypeModifier.Extern },
            { SyntaxKind.ConstKeyword, TypeModifier.Const },
            { SyntaxKind.EventKeyword, TypeModifier.Event },
            { SyntaxKind.FixedKeyword, TypeModifier.Fixed },
            { SyntaxKind.ReadOnlyKeyword, TypeModifier.ReadOnly },
            { SyntaxKind.RefKeyword, TypeModifier.Ref },
            { SyntaxKind.InKeyword, TypeModifier.In },
            { SyntaxKind.OutKeyword, TypeModifier.Out },
            { SyntaxKind.ParamsKeyword, TypeModifier.Params },
            { SyntaxKind.ThisKeyword, TypeModifier.This },
            { SyntaxKind.ScopedKeyword, TypeModifier.Scoped },
            { SyntaxKind.UnsafeKeyword, TypeModifier.Unsafe },
            { SyntaxKind.VolatileKeyword, TypeModifier.Volatile },
            { SyntaxKind.AsyncKeyword, TypeModifier.Async },
            { SyntaxKind.PartialKeyword, TypeModifier.Partial },
            { SyntaxKind.RequiredKeyword, TypeModifier.Required },
        };

        public static ImmutableEquatableArray<TypeModifier> Extract(ITypeSymbol typeSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var extractedModifiers = new List<TypeModifier>();

            foreach (SyntaxReference syntaxReference in typeSymbol.DeclaringSyntaxReferences)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                SyntaxNode syntaxNode = syntaxReference.GetSyntax(context.CancellationToken);
                switch (syntaxNode)
                {
                    case BaseTypeDeclarationSyntax baseTypeDeclaration:
                        AddModifiers(baseTypeDeclaration.Modifiers, extractedModifiers);
                        break;
                    case DelegateDeclarationSyntax delegateDeclaration:
                        AddModifiers(delegateDeclaration.Modifiers, extractedModifiers);
                        break;
                }
            }

            return extractedModifiers.ToImmutableEquatableArray();
        }

        private static void AddModifiers(SyntaxTokenList modifiers, List<TypeModifier> extractedModifiers)
        {
            foreach (SyntaxToken modifierToken in modifiers)
            {
                if (TryMapModifier(modifierToken.Kind(), out TypeModifier modifier) && !extractedModifiers.Contains(modifier))
                {
                    extractedModifiers.Add(modifier);
                }
            }
        }

        private static bool TryMapModifier(SyntaxKind modifierKind, out TypeModifier modifier)
        {
            return s_modifiers.TryGetValue(modifierKind, out modifier);
        }
    }
}
