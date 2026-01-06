using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using Tycho.Utils.SourceGenerator.Model;
using Tycho.Utils.SourceGenerator.Model.Partial;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator
{
    [Generator]
    public sealed class TychoSourceGenerator : IIncrementalGenerator
    {
        private const string TychoDefinitionAttribute = "Tycho.TychoDefinitionAttribute";
        private const string TychoAppBaseClass = "Tycho.Apps.TychoApp";
        private const string TychoModuleBaseClass = "Tycho.Modules.TychoModule";

        private static readonly string AppDefinitionTemplate = EmbeddedResource.GetContent("Templates/AppDefinition.sbncs");

        private static readonly string AppFacadeTemplate = EmbeddedResource.GetContent("Templates/AppFacade.sbncs");

        private static readonly string AppSetupTemplate = EmbeddedResource.GetContent("Templates/AppSetup.sbncs");

        private static readonly string ModuleDefinitionTemplate = EmbeddedResource.GetContent("Templates/ModuleDefinition.sbncs");

        private static readonly string EventDispatcherTemplate = EmbeddedResource.GetContent("Templates/EventDispatcher.sbncs");

        private static readonly MethodSignatureModel DefineAppEventsMethodSignature = new MethodSignatureModel(
            methodName: "DefineEvents",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                new TypeModel("Tycho.Apps", ImmutableEquatableArray<string>.Empty, "IAppEvents"),
            }),
            result: new TypeModel("System", ImmutableEquatableArray<string>.Empty, "Void"));

        private static readonly MethodSignatureModel DefineModuleEventsMethodSignature = new MethodSignatureModel(
            methodName: "DefineEvents",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                new TypeModel("Tycho.Modules", ImmutableEquatableArray<string>.Empty, "IModuleEvents"),
            }),
            result: new TypeModel("System", ImmutableEquatableArray<string>.Empty, "Void"));

        private static readonly MethodSignatureModel AppHandlesMethodSignature = new MethodSignatureModel(
            methodName: "Handles",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: new TypeModel("Tycho.Apps", ImmutableEquatableArray<string>.Empty, "IAppEvents"));

        private static readonly MethodSignatureModel ModuleHandlesMethodSignature = new MethodSignatureModel(
            methodName: "Handles",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: new TypeModel("Tycho.Modules", ImmutableEquatableArray<string>.Empty, "IModuleEvents"));

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var getTychoDefinitionTypeSymbolStepResult = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: TychoDefinitionAttribute,
                predicate: GetTychoDefinitionClassStepPredicate,
                transform: GetTychoDefinitionClassStepTransform);

            var getTychoDefinitionModelStepResult = getTychoDefinitionTypeSymbolStepResult
                .Select(GetTychoDefinitionModelStepTransform);

            var getTychoFacadeModelStepResult = getTychoDefinitionTypeSymbolStepResult
                .Select(GetTychoFacadeModelStepTransform);

            var getTychoSetupModelStepResult = getTychoDefinitionTypeSymbolStepResult
                .Select(GetTychoSetupModelStepTransform);

            var getDefineEventsMethodDefinitionsStepResult = getTychoDefinitionTypeSymbolStepResult
                .SelectMany(GetDefineEventsMethodDefinitionsStepTransform);

            var getHandlesMethodInvocationsStepResult = getDefineEventsMethodDefinitionsStepResult
                .Select(GetHandlesMethodInvocationsStepTransform);

            var getEventDispatcherModelStepResult = getHandlesMethodInvocationsStepResult
                .Select(GetEventDispatcherModelStepTransform);

            context.RegisterSourceOutput(
                getEventDispatcherModelStepResult,
                (outputContext, model) =>
                {
                    GenerateSourceFromTemplate(
                        outputContext,
                        model,
                        EventDispatcherTemplate,
                        $"{model.DefinitionType}.EventDispatcher.g.cs");
                });

            context.RegisterSourceOutput(
                getTychoFacadeModelStepResult,
                (outputContext, model) =>
                {
                    //if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    GenerateSourceFromTemplate(
                        outputContext,
                        model,
                        AppFacadeTemplate,
                        $"{model.DefinitionType}.Facade.g.cs");
                });

            context.RegisterSourceOutput(
                getTychoDefinitionModelStepResult,
                (outputContext, model) =>
                {
                    if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    GenerateSourceFromTemplate(
                        outputContext,
                        model,
                        ChooseTemplate(model.DefinitionKind, AppDefinitionTemplate, ModuleDefinitionTemplate),
                        $"{model.DefinitionType}.g.cs");
                });

            context.RegisterSourceOutput(
                getTychoSetupModelStepResult,
                (outputContext, model) =>
                {
                    //if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    GenerateSourceFromTemplate(
                        outputContext,
                        model,
                        AppSetupTemplate,
                        $"{model.DefinitionType}.Setup.g.cs");
                });
        }

        private static bool GetTychoDefinitionClassStepPredicate(SyntaxNode node, CancellationToken token)
        {
            return node is ClassDeclarationSyntax;
        }

        private static (TychoDefinitionKind, ClassDefinitionModel) GetTychoDefinitionClassStepTransform(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var definitionKind = GetDefinitionKind(context, token);
            var classType = GetTypeModel(context.TargetSymbol);
            var methods = GetMethodDefinitionModels(context, classType, token);

            return (definitionKind, new ClassDefinitionModel(classType, methods));
        }

        private static TychoDefinitionModel GetTychoDefinitionModelStepTransform((TychoDefinitionKind Kind, ClassDefinitionModel Model) input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoDefinitionModel(input.Model.ClassType, input.Kind);
        }

        private static TychoFacadeModel GetTychoFacadeModelStepTransform((TychoDefinitionKind Kind, ClassDefinitionModel Model) input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoFacadeModel(input.Model.ClassType);
        }

        private static TychoSetupModel GetTychoSetupModelStepTransform((TychoDefinitionKind Kind, ClassDefinitionModel Model) input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoSetupModel(input.Model.ClassType);
        }

        private static ImmutableEquatableArray<MethodDefinitionModel> GetDefineEventsMethodDefinitionsStepTransform((TychoDefinitionKind, ClassDefinitionModel Model) input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return input.Model.Methods
                .Where(method =>
                    method.Signature == DefineAppEventsMethodSignature ||
                    method.Signature == DefineModuleEventsMethodSignature)
                .ToImmutableEquatableArray();
        }

        private static (TypeModel, ImmutableEquatableArray<MethodInvocationModel>) GetHandlesMethodInvocationsStepTransform(MethodDefinitionModel model, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = model.Body
                .Where(invocation =>
                    invocation.Signature == AppHandlesMethodSignature ||
                    invocation.Signature == ModuleHandlesMethodSignature)
                .ToImmutableEquatableArray();
            return (model.ContainingType, invocations);
        }

        private static EventDispatcherModel GetEventDispatcherModelStepTransform(
            (TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocation) input, 
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new EventDispatcherModel(
                input.DefinitionType,
                input.MethodInvocation
                    .Select(methodInvocationModel => methodInvocationModel.TypeArguments.First())
                    .ToImmutableEquatableArray());
        }

        private static TychoDefinitionKind GetDefinitionKind(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!(context.TargetSymbol is ITypeSymbol typeSymbol))
            {
                return TychoDefinitionKind.Unknown;
            }

            var compilation = context.SemanticModel.Compilation;
            var tychoAppSymbol = compilation.GetTypeByMetadataName(TychoAppBaseClass);
            var tychoModuleSymbol = compilation.GetTypeByMetadataName(TychoModuleBaseClass);

            if (tychoAppSymbol != null && InheritsFrom(typeSymbol, tychoAppSymbol))
            {
                return TychoDefinitionKind.App;
            }

            if (tychoModuleSymbol != null && InheritsFrom(typeSymbol, tychoModuleSymbol))
            {
                return TychoDefinitionKind.Module;
            }

            return TychoDefinitionKind.Unknown;
        }

        private static ImmutableEquatableArray<MethodDefinitionModel> GetMethodDefinitionModels(GeneratorAttributeSyntaxContext context, TypeModel containingType, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!(context.TargetSymbol is INamespaceOrTypeSymbol classSymbol))
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

        private static ImmutableEquatableArray<MethodInvocationModel> GetMethodBody(GeneratorAttributeSyntaxContext context, IMethodSymbol methodSymbol, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var methodInvocations = new HashSet<MethodInvocationModel>();

            foreach (var syntaxRef in methodSymbol.DeclaringSyntaxReferences)
            {
                token.ThrowIfCancellationRequested();

                if (!(syntaxRef.GetSyntax(token) is MethodDeclarationSyntax methodSyntax) || methodSyntax.Body == null)
                {
                    continue;
                }

                var semanticModel = context.SemanticModel.Compilation.GetSemanticModel(methodSyntax.SyntaxTree);

                foreach (var invocationSyntax in methodSyntax.Body.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    token.ThrowIfCancellationRequested();

                    var symbolInfo = semanticModel.GetSymbolInfo(invocationSyntax, token);
                    var symbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();

                    if (!(symbol is IMethodSymbol invokedMethodSymbol))
                    {
                        continue;
                    }

                    methodInvocations.Add(new MethodInvocationModel(
                        GetMethodSignatureModel(invokedMethodSymbol),
                        invokedMethodSymbol.TypeArguments
                            .Select(GetTypeModel)
                            .ToImmutableEquatableArray()));
                }
            }

            return methodInvocations.ToImmutableEquatableArray();
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

        private static bool InheritsFrom(ITypeSymbol type, ITypeSymbol baseType)
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

            return new TypeModel(
                typeNamespace,
                containingTypes.ToImmutableEquatableArray(), 
                symbol.Name);
        }

        private static string ChooseTemplate(TychoDefinitionKind kind, string appTemplate, string moduleTemplate)
        {
            return kind switch
            {
                TychoDefinitionKind.App => appTemplate,
                TychoDefinitionKind.Module => moduleTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), $"Unsupported definition kind: {kind}"),
            };
        }

        private static void GenerateSourceFromTemplate(
            SourceProductionContext context,
            object model,
            string templateContent,
            string targetFileName)
        {
            var template = Template.Parse(templateContent);
            var output = template.Render(model);
            var sourceText = SourceText.From(output, Encoding.UTF8);
            context.AddSource(targetFileName, sourceText);
        }
    }
}
