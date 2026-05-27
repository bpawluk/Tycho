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
            switch (modifierKind)
            {
                case SyntaxKind.NewKeyword:
                    modifier = TypeModifier.New;
                    return true;
                case SyntaxKind.PublicKeyword:
                    modifier = TypeModifier.Public;
                    return true;
                case SyntaxKind.ProtectedKeyword:
                    modifier = TypeModifier.Protected;
                    return true;
                case SyntaxKind.InternalKeyword:
                    modifier = TypeModifier.Internal;
                    return true;
                case SyntaxKind.PrivateKeyword:
                    modifier = TypeModifier.Private;
                    return true;
                case SyntaxKind.FileKeyword:
                    modifier = TypeModifier.File;
                    return true;
                case SyntaxKind.StaticKeyword:
                    modifier = TypeModifier.Static;
                    return true;
                case SyntaxKind.VirtualKeyword:
                    modifier = TypeModifier.Virtual;
                    return true;
                case SyntaxKind.SealedKeyword:
                    modifier = TypeModifier.Sealed;
                    return true;
                case SyntaxKind.OverrideKeyword:
                    modifier = TypeModifier.Override;
                    return true;
                case SyntaxKind.AbstractKeyword:
                    modifier = TypeModifier.Abstract;
                    return true;
                case SyntaxKind.ExternKeyword:
                    modifier = TypeModifier.Extern;
                    return true;
                case SyntaxKind.ConstKeyword:
                    modifier = TypeModifier.Const;
                    return true;
                case SyntaxKind.EventKeyword:
                    modifier = TypeModifier.Event;
                    return true;
                case SyntaxKind.FixedKeyword:
                    modifier = TypeModifier.Fixed;
                    return true;
                case SyntaxKind.ReadOnlyKeyword:
                    modifier = TypeModifier.ReadOnly;
                    return true;
                case SyntaxKind.RefKeyword:
                    modifier = TypeModifier.Ref;
                    return true;
                case SyntaxKind.InKeyword:
                    modifier = TypeModifier.In;
                    return true;
                case SyntaxKind.OutKeyword:
                    modifier = TypeModifier.Out;
                    return true;
                case SyntaxKind.ParamsKeyword:
                    modifier = TypeModifier.Params;
                    return true;
                case SyntaxKind.ThisKeyword:
                    modifier = TypeModifier.This;
                    return true;
                case SyntaxKind.ScopedKeyword:
                    modifier = TypeModifier.Scoped;
                    return true;
                case SyntaxKind.UnsafeKeyword:
                    modifier = TypeModifier.Unsafe;
                    return true;
                case SyntaxKind.VolatileKeyword:
                    modifier = TypeModifier.Volatile;
                    return true;
                case SyntaxKind.AsyncKeyword:
                    modifier = TypeModifier.Async;
                    return true;
                case SyntaxKind.PartialKeyword:
                    modifier = TypeModifier.Partial;
                    return true;
                case SyntaxKind.RequiredKeyword:
                    modifier = TypeModifier.Required;
                    return true;
                default:
                    modifier = default;
                    return false;
            }
        }
    }
}
