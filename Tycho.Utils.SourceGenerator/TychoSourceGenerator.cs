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
            IncrementalValuesProvider<(TychoDefinitionKind, ClassDefinitionModel)> tychoPipelineBase = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: TychoDefinitionAttributeReference.FullName,
                predicate: GetTychoPipelineBasePredicate,
                transform: GetTychoPipelineBaseTransform);

            context.AddTychoFacadePipeline(tychoPipelineBase)
                   .AddTychoPublisherPipeline(tychoPipelineBase)
                     .AddTychoEventSerializerPipeline(tychoPipelineBase)
                   .AddTychoParentPipeline(tychoPipelineBase)
                   .AddTychoSetupPipeline(tychoPipelineBase)
                   .AddTychoExtensionsPipeline(tychoPipelineBase);
        }

        private static bool GetTychoPipelineBasePredicate(SyntaxNode node, CancellationToken token)
        {
            return node is ClassDeclarationSyntax;
        }

        private static (TychoDefinitionKind, ClassDefinitionModel) GetTychoPipelineBaseTransform(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            TychoDefinitionKind definitionKind = GetDefinitionKind(context, token);
            TypeModel definitionType = GetDefinitionType(context.TargetSymbol, token);
            ImmutableEquatableArray<MethodDefinitionModel> methods = GetMethodDefinitionModels(context, definitionType, token);
            return (definitionKind, new ClassDefinitionModel(definitionType, methods));
        }

        private static TychoDefinitionKind GetDefinitionKind(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!(context.TargetSymbol is ITypeSymbol typeSymbol))
            {
                return TychoDefinitionKind.Unknown;
            }

            Compilation compilation = context.SemanticModel.Compilation;
            INamedTypeSymbol tychoAppSymbol = compilation.GetTypeByMetadataName(TychoAppReference.FullName);
            if (tychoAppSymbol != null && TypeInheritsFrom(typeSymbol, tychoAppSymbol))
            {
                return TychoDefinitionKind.App;
            }

            INamedTypeSymbol tychoModuleSymbol = compilation.GetTypeByMetadataName(TychoModuleReference.FullName);
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

            foreach (IMethodSymbol methodSymbol in classSymbol.GetMembers().OfType<IMethodSymbol>())
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
            for (ITypeSymbol current = type; current != null; current = current.BaseType)
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
            string methodName = methodSymbol.Name;
            TypeModel returnType = GetTypeModel(methodSymbol.ReturnType);
            var parameters = methodSymbol.Parameters
                .Select(paramSymbol => GetTypeModel(paramSymbol.Type))
                .ToImmutableEquatableArray();
            return new MethodSignatureModel(methodName, parameters, returnType);
        }

        private static ImmutableEquatableArray<MethodInvocationModel> GetMethodBody(GeneratorAttributeSyntaxContext context, IMethodSymbol methodSymbol, CancellationToken token)
        {
            var methodInvocations = new HashSet<MethodInvocationModel>();
            var visitedMethods = new HashSet<IMethodSymbol>(SymbolEqualityComparer.Default);
            CollectMethodInvocations(
                context.SemanticModel.Compilation,
                methodSymbol,
                methodInvocations,
                visitedMethods,
                token);
            return methodInvocations.ToImmutableEquatableArray();
        }

        private static void CollectMethodInvocations(
            Compilation compilation,
            IMethodSymbol methodSymbol,
            HashSet<MethodInvocationModel> methodInvocations,
            HashSet<IMethodSymbol> visitedMethods,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            IMethodSymbol traversedMethodSymbol = methodSymbol.ReducedFrom ?? methodSymbol;
            if (!visitedMethods.Add(traversedMethodSymbol))
            {
                return;
            }

            foreach (SyntaxReference syntaxRef in traversedMethodSymbol.DeclaringSyntaxReferences)
            {
                token.ThrowIfCancellationRequested();
                if (!(syntaxRef.GetSyntax(token) is MethodDeclarationSyntax methodSyntax))
                {
                    continue;
                }

                SemanticModel semanticModel = compilation.GetSemanticModel(methodSyntax.SyntaxTree);

                IEnumerable<InvocationExpressionSyntax> invocationExpressions =
                    (methodSyntax.Body?.DescendantNodes().OfType<InvocationExpressionSyntax>() ?? Enumerable.Empty<InvocationExpressionSyntax>())
                        .Concat(methodSyntax.ExpressionBody?.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>() ?? Enumerable.Empty<InvocationExpressionSyntax>());

                foreach (InvocationExpressionSyntax invocationSyntax in invocationExpressions)
                {
                    token.ThrowIfCancellationRequested();
                    SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(invocationSyntax, token);
                    ISymbol symbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();

                    if (!(symbol is IMethodSymbol invokedMethodSymbol))
                    {
                        continue;
                    }

                    IMethodSymbol methodInvocationSymbol = invokedMethodSymbol.ReducedFrom ?? invokedMethodSymbol;
                    methodInvocations.Add(
                        new MethodInvocationModel(
                            GetMethodSignatureModel(methodInvocationSymbol),
                            invokedMethodSymbol.ReceiverType is ISymbol receiverSymbol
                                ? GetTypeModel(receiverSymbol)
                                : default,
                            invokedMethodSymbol.TypeParameters
                                .Zip(invokedMethodSymbol.TypeArguments, GetTypeArgumentModel)
                                .ToImmutableEquatableArray()));

                    if (methodInvocationSymbol.DeclaringSyntaxReferences.Length > 0)
                    {
                        CollectMethodInvocations(
                            compilation,
                            methodInvocationSymbol,
                            methodInvocations,
                            visitedMethods,
                            token);
                    }
                }
            }
        }

        private static TypeModel GetTypeModel(ISymbol symbol)
        {
            string typeNamespace = symbol
                .ContainingNamespace
                .ToDisplayString(SymbolDisplayFormat
                    .FullyQualifiedFormat
                    .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));

            if (symbol is INamedTypeSymbol namedSymbol)
            {
                var containingTypes = new Stack<ContainingTypeModel>();

                for (INamedTypeSymbol containingTypeSymbol = symbol.ContainingType;
                    containingTypeSymbol != null;
                    containingTypeSymbol = containingTypeSymbol.ContainingType)
                {
                    containingTypes.Push(new ContainingTypeModel(
                        containingTypeSymbol.Name,
                        GetTypeParameters(containingTypeSymbol),
                        GetTypeParameterConstraintClauses(containingTypeSymbol),
                        GetTypeArguments(containingTypeSymbol)));
                }

                return new TypeModel(
                    typeNamespace,
                    containingTypes.ToImmutableEquatableArray(),
                    namedSymbol.Name,
                    GetTypeParameters(namedSymbol),
                    GetTypeParameterConstraintClauses(namedSymbol),
                    GetTypeArguments(namedSymbol));
            }

            string typeName = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            return new TypeModel(
                typeNamespace,
                ImmutableEquatableArray<ContainingTypeModel>.Empty,
                typeName,
                ImmutableEquatableArray<string>.Empty,
                ImmutableEquatableArray<string>.Empty,
                ImmutableEquatableArray<string>.Empty);
        }

        private static ImmutableEquatableArray<string> GetTypeParameters(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeParameters.Length == 0)
            {
                return ImmutableEquatableArray<string>.Empty;
            }

            return typeSymbol.TypeParameters
                .Select(parameter => parameter.Name)
                .ToImmutableEquatableArray();
        }

        private static ImmutableEquatableArray<string> GetTypeArguments(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeArguments.Length == 0)
            {
                return ImmutableEquatableArray<string>.Empty;
            }

            return typeSymbol.TypeArguments
                .Select(GetTypeArgumentReferenceName)
                .ToImmutableEquatableArray();
        }

        private static string GetTypeArgumentReferenceName(ITypeSymbol typeArgument)
        {
            if (typeArgument is ITypeParameterSymbol typeParameter)
            {
                return typeParameter.Name;
            }

            return typeArgument.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }

        private static ImmutableEquatableArray<string> GetTypeParameterConstraintClauses(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeParameters.Length == 0)
            {
                return ImmutableEquatableArray<string>.Empty;
            }

            return typeSymbol.TypeParameters
                .Select(GetTypeParameterConstraintClause)
                .Where(clause => !string.IsNullOrWhiteSpace(clause))
                .ToImmutableEquatableArray();
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

        private static TypeArgument GetTypeArgumentModel(ITypeParameterSymbol typeParameter, ITypeSymbol typeArgument)
        {
            return new TypeArgument(typeParameter.Name, GetTypeModel(typeArgument));
        }
    }
}
