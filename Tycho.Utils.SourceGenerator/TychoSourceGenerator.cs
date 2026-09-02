using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Tycho.Utils.SourceGenerator.Extractors;
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
                   .AddTychoAppBuilderPipeline(tychoPipelineBase)
                   .AddTychoExtensionsPipeline(tychoPipelineBase);
        }

        private static bool GetTychoPipelineBasePredicate(SyntaxNode node, CancellationToken token)
        {
            return node is ClassDeclarationSyntax;
        }

        private static (TychoDefinitionKind, ClassDefinitionModel) GetTychoPipelineBaseTransform(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (context.TargetSymbol is ITypeSymbol targetTypeSymbol)
            {
                Compilation compilation = context.SemanticModel.Compilation;
                SemanticModelProvider semanticModelProvider = new SemanticModelProvider(compilation);
                ExtractorContext extractorContext = new ExtractorContext(compilation, semanticModelProvider, cancellationToken);

                TychoDefinitionKind definitionKind = TychoDefinitionKindExtractor.Extract(targetTypeSymbol, extractorContext);
                TypeDefinitionModel definitionType = TypeDefinitionModelExtractor.Extract(targetTypeSymbol, extractorContext);

                ImmutableEquatableArray<MethodDefinitionModel> methodDefinitions = targetTypeSymbol
                    .GetMembers()
                    .OfType<IMethodSymbol>()
                    .Where(methodSymbol =>
                        methodSymbol.Name == TychoAppReference.DefineContractMethodSignature.MethodName ||
                        methodSymbol.Name == TychoAppReference.DefineEventsMethodSignature.MethodName ||
                        methodSymbol.Name == TychoAppReference.IncludeModulesMethodSignature.MethodName ||
                        methodSymbol.Name == TychoModuleReference.DefineContractMethodSignature.MethodName ||
                        methodSymbol.Name == TychoModuleReference.DefineEventsMethodSignature.MethodName ||
                        methodSymbol.Name == TychoModuleReference.IncludeModulesMethodSignature.MethodName)
                    .Select(methodSymbol => MethodDefinitionExtractor.Extract(definitionType, methodSymbol, extractorContext))
                    .ToImmutableEquatableArray();

                return (definitionKind, new ClassDefinitionModel(definitionType, methodDefinitions));
            }

            return (TychoDefinitionKind.Unknown, default);
        }
    }
}
