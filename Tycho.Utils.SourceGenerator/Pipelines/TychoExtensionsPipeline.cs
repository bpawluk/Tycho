using System.Threading;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Extensions;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.TemplateModels;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Pipelines
{
    internal static class TychoExtensionsPipeline
    {
        private static readonly string s_appExtensionsTemplate = EmbeddedResource.GetContent("Templates/AppExtensions.sbncs");

        public static IncrementalGeneratorInitializationContext AddTychoExtensionsPipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind Kind, TypeDefinitionModel DefinitionType)> pipelineBase)
        {
            IncrementalValuesProvider<TychoExtensionsModel> getTychoExtensionsModelStepResult = pipelineBase
                .Where(GetTychoExtensionsModelStepPredicate)
                .Select(GetTychoExtensionsModelStepTransform)
                .WithTrackingName("TychoExtensions.Model");

            context.RegisterSourceOutput(
                getTychoExtensionsModelStepResult,
                (outputContext, model) =>
                {
                    outputContext.GenerateSourceFromTemplate(
                        new AppExtensionsTM(model),
                        s_appExtensionsTemplate,
                        $"{model.DefinitionType.FullMetadataName}.Extensions.g.cs");
                });

            return context;
        }

        private static bool GetTychoExtensionsModelStepPredicate((TychoDefinitionKind Kind, TypeDefinitionModel DefinitionType) input)
        {
            return input.Kind == TychoDefinitionKind.App;
        }

        private static TychoExtensionsModel GetTychoExtensionsModelStepTransform(
            (TychoDefinitionKind Kind, TypeDefinitionModel DefinitionType) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoExtensionsModel(input.DefinitionType);
        }
    }
}
