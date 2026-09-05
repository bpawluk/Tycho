using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.Models;
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
            IncrementalValuesProvider<TychoDefinitionModel> tychoPipelineBase = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: TychoDefinitionAttributeReference.FullName,
                predicate: GetTychoPipelineBasePredicate,
                transform: GetTychoPipelineBaseTransform)
                .Where(model => model.IsValid);

            IncrementalValuesProvider<(TychoDefinitionKind Kind, TypeDefinitionModel DefinitionType)> definitionTypes = tychoPipelineBase
                .Select(GetDefinitionType)
                .WithTrackingName("TychoDefinition.Type");

            IncrementalValuesProvider<(TychoDefinitionKind Kind, MethodDefinitionModel Method)> contractDefinitions = tychoPipelineBase
                .Select(GetContractDefinition)
                .WithTrackingName("TychoDefinition.Contract");

            IncrementalValuesProvider<(TychoDefinitionKind Kind, MethodDefinitionModel Method)> eventDefinitions = tychoPipelineBase
                .Select(GetEventDefinition)
                .WithTrackingName("TychoDefinition.Events");

            IncrementalValuesProvider<(TychoDefinitionKind Kind, MethodDefinitionModel Method)> structureDefinitions = tychoPipelineBase
                .Select(GetStructureDefinition)
                .WithTrackingName("TychoDefinition.Structure");

            context.AddTychoFacadePipeline(contractDefinitions)
                   .AddTychoPublisherPipeline(eventDefinitions)
                   .AddTychoEventSerializerPipeline(eventDefinitions)
                   .AddTychoParentPipeline(contractDefinitions)
                   .AddTychoSetupPipeline(structureDefinitions)
                   .AddTychoAppBuilderPipeline(definitionTypes)
                   .AddTychoExtensionsPipeline(definitionTypes);
        }

        private static (TychoDefinitionKind Kind, TypeDefinitionModel DefinitionType) GetDefinitionType(
            TychoDefinitionModel model,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return (model.DefinitionKind, model.DefinitionType);
        }

        private static (TychoDefinitionKind Kind, MethodDefinitionModel Method) GetContractDefinition(
            TychoDefinitionModel model,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return (model.DefinitionKind, model.DefineContractMethod);
        }

        private static (TychoDefinitionKind Kind, MethodDefinitionModel Method) GetEventDefinition(
            TychoDefinitionModel model,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return (model.DefinitionKind, model.DefineEventsMethod);
        }

        private static (TychoDefinitionKind Kind, MethodDefinitionModel Method) GetStructureDefinition(
            TychoDefinitionModel model,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return (model.DefinitionKind, model.IncludeModulesMethod);
        }

        private static bool GetTychoPipelineBasePredicate(SyntaxNode node, CancellationToken token)
        {
            return node is ClassDeclarationSyntax;
        }

        private static TychoDefinitionModel GetTychoPipelineBaseTransform(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (context.TargetSymbol is ITypeSymbol targetTypeSymbol)
            {
                Compilation compilation = context.SemanticModel.Compilation;
                SemanticModelProvider semanticModelProvider = new SemanticModelProvider(compilation);
                ExtractorContext extractorContext = new ExtractorContext(compilation, semanticModelProvider, cancellationToken);

                TychoDefinitionKind definitionKind = TychoDefinitionKindExtractor.Extract(targetTypeSymbol, extractorContext);
                if (definitionKind == TychoDefinitionKind.Unknown || targetTypeSymbol.IsAbstract)
                {
                    return TychoDefinitionModel.None();
                }

                TypeDefinitionModel definitionType = TypeDefinitionModelExtractor.Extract(targetTypeSymbol, extractorContext);

                INamedTypeSymbol baseTypeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(definitionKind == TychoDefinitionKind.App ? TychoAppReference.FullName : TychoModuleReference.FullName);
                if (baseTypeSymbol == null ||
                    !TryGetRequiredMethod(targetTypeSymbol, baseTypeSymbol, "DefineContract", out IMethodSymbol defineContractMethod) ||
                    !TryGetRequiredMethod(targetTypeSymbol, baseTypeSymbol, "DefineEvents", out IMethodSymbol defineEventsMethod) ||
                    !TryGetRequiredMethod(targetTypeSymbol, baseTypeSymbol, "IncludeModules", out IMethodSymbol includeModulesMethod))
                {
                    return TychoDefinitionModel.None();
                }

                return new TychoDefinitionModel(
                    definitionKind,
                    definitionType,
                    MethodDefinitionExtractor.Extract(definitionType, defineContractMethod, extractorContext),
                    MethodDefinitionExtractor.Extract(definitionType, defineEventsMethod, extractorContext),
                    MethodDefinitionExtractor.Extract(definitionType, includeModulesMethod, extractorContext));
            }

            return default;
        }

        private static bool TryGetRequiredMethod(
            ITypeSymbol targetTypeSymbol,
            INamedTypeSymbol baseTypeSymbol,
            string methodName,
            out IMethodSymbol requiredMethod)
        {
            requiredMethod = null;

            IMethodSymbol baseMethod = baseTypeSymbol.GetMembers(methodName).OfType<IMethodSymbol>().SingleOrDefault();
            if (baseMethod == null)
            {
                return false;
            }

            for (ITypeSymbol currentType = targetTypeSymbol; currentType != null; currentType = currentType.BaseType)
            {
                foreach (IMethodSymbol candidate in currentType.GetMembers(methodName).OfType<IMethodSymbol>())
                {
                    if (!candidate.IsAbstract && Overrides(candidate, baseMethod))
                    {
                        requiredMethod = candidate;
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool Overrides(IMethodSymbol candidate, IMethodSymbol baseMethod)
        {
            for (IMethodSymbol current = candidate; current != null; current = current.OverriddenMethod)
            {
                if (SymbolEqualityComparer.Default.Equals(current.OriginalDefinition, baseMethod.OriginalDefinition))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
