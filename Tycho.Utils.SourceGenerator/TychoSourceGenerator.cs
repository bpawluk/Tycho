using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.Pipelines;
using Tycho.Utils.SourceGenerator.References.Tycho;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator
{
    [Generator]
    public sealed class TychoSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var tychoPipelineBase = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: TychoDefinitionAttributeReference.FullName,
                predicate: GetTychoPipelineBasePredicate,
                transform: GetTychoPipelineBaseTransform);

            context.AddTychoDefinitionPipeline(tychoPipelineBase)
                   .AddTychoExtensionsPipeline(tychoPipelineBase)
                   .AddTychoFacadePipeline(tychoPipelineBase)
                   .AddTychoParentPipeline(tychoPipelineBase)
                   .AddTychoPublisherPipeline(tychoPipelineBase);
        }

        private static bool GetTychoPipelineBasePredicate(SyntaxNode node, CancellationToken token)
        {
            return node is ClassDeclarationSyntax;
        }

        private static (TychoDefinitionKind, ClassDefinitionModel) GetTychoPipelineBaseTransform(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var definitionKind = GetDefinitionKind(context, token);
            var definitionType = GetDefinitionType(context.TargetSymbol, token);
            var methods = GetMethodDefinitionModels(context, definitionType, token);
            return (definitionKind, new ClassDefinitionModel(definitionType, methods));
        }

        private static TychoDefinitionKind GetDefinitionKind(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!(context.TargetSymbol is ITypeSymbol typeSymbol))
            {
                return TychoDefinitionKind.Unknown;
            }

            var compilation = context.SemanticModel.Compilation;
            var tychoAppSymbol = compilation.GetTypeByMetadataName(TychoAppReference.FullName);
            if (tychoAppSymbol != null && TypeInheritsFrom(typeSymbol, tychoAppSymbol))
            {
                return TychoDefinitionKind.App;
            }

            var tychoModuleSymbol = compilation.GetTypeByMetadataName(TychoModuleReference.FullName);
            if (tychoModuleSymbol != null && TypeInheritsFrom(typeSymbol, tychoModuleSymbol))
            {
                return TychoDefinitionKind.Module;
            }

            return TychoDefinitionKind.Unknown;
        }

        private static TypeModel GetDefinitionType(ISymbol symbol, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return GetTypeModel(symbol);
        }

        private static ImmutableEquatableArray<MethodDefinitionModel> GetMethodDefinitionModels(
            GeneratorAttributeSyntaxContext context, 
            TypeModel containingType, 
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!(context.TargetSymbol is ITypeSymbol classSymbol))
            {
                return ImmutableEquatableArray<MethodDefinitionModel>.Empty;
            }

            var methodModels = new HashSet<MethodDefinitionModel>();

            foreach (var methodSymbol in classSymbol.GetMembers().OfType<IMethodSymbol>())
            {
                token.ThrowIfCancellationRequested();

                if (!methodSymbol.IsOverride)
                {
                    continue;
                }

                var methodModel = new MethodDefinitionModel(
                    containingType,
                    GetMethodSignatureModel(methodSymbol),
                    GetMethodBody(context, methodSymbol, token));

                methodModels.Add(methodModel);
            }

            return methodModels.ToImmutableEquatableArray();
        }
        private static bool TypeInheritsFrom(ITypeSymbol type, ITypeSymbol baseType)
        {
            for (var current = type; current != null; current = current.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(current, baseType))
                {
                    return true;
                }
            }
            return false;
        }

        private static MethodSignatureModel GetMethodSignatureModel(IMethodSymbol methodSymbol)
        {
            var methodName = methodSymbol.Name;
            var returnType = GetTypeModel(methodSymbol.ReturnType);
            var parameters = methodSymbol.Parameters
                .Select(paramSymbol => GetTypeModel(paramSymbol.Type))
                .ToImmutableEquatableArray();
            return new MethodSignatureModel(methodName, parameters, returnType);
        }

        private static ImmutableEquatableArray<MethodInvocationModel> GetMethodBody(GeneratorAttributeSyntaxContext context, IMethodSymbol methodSymbol, CancellationToken token)
        {
            var methodInvocations = new HashSet<MethodInvocationModel>();
            foreach (var syntaxRef in methodSymbol.DeclaringSyntaxReferences)
            {
                if (!(syntaxRef.GetSyntax(token) is MethodDeclarationSyntax methodSyntax) || methodSyntax.Body == null)
                {
                    continue;
                }
                var semanticModel = context.SemanticModel.Compilation.GetSemanticModel(methodSyntax.SyntaxTree);

                foreach (var invocationSyntax in methodSyntax.Body.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    var symbolInfo = semanticModel.GetSymbolInfo(invocationSyntax, token);
                    var symbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();

                    if (!(symbol is IMethodSymbol invokedMethodSymbol))
                    {
                        continue;
                    }

                    methodInvocations.Add(
                        new MethodInvocationModel(
                            GetMethodSignatureModel(invokedMethodSymbol),
                            invokedMethodSymbol.ReceiverType is ISymbol receiverSymbol
                                ? GetTypeModel(receiverSymbol)
                                : default,
                            invokedMethodSymbol.TypeParameters
                                .Zip(invokedMethodSymbol.TypeArguments, GetTypeArgumentModel)
                                .ToImmutableEquatableArray()));
                }
            }

            return methodInvocations.ToImmutableEquatableArray();
        }

        private static TypeModel GetTypeModel(ISymbol symbol)
        {
            var typeNamespace = symbol
                .ContainingNamespace
                .ToDisplayString(SymbolDisplayFormat
                    .FullyQualifiedFormat
                    .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));

            var containingTypes = new Stack<string>();
            for (var current = symbol.ContainingType; current != null; current = current.ContainingType)
            {
                containingTypes.Push(current.Name);
            }

            return new TypeModel(typeNamespace, containingTypes.ToImmutableEquatableArray(), symbol.Name);
        }

        private static TypeArgument GetTypeArgumentModel(ITypeParameterSymbol typeParameter, ITypeSymbol typeArgument)
        {
            return new TypeArgument(typeParameter.Name, GetTypeModel(typeArgument));
        }
    }
}
