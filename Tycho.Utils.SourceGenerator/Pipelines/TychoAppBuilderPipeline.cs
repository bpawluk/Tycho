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
    internal static class TychoAppBuilderPipeline
    {
        private static readonly string s_appBuilderTemplate = EmbeddedResource.GetContent("Templates/AppBuilder.sbncs");

        public static IncrementalGeneratorInitializationContext AddTychoAppBuilderPipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind, ClassDefinitionModel)> pipelineBase)
        {
            IncrementalValuesProvider<TychoAppBuilderModel> getTychoAppBuilderModelStepResult = pipelineBase
                .Where(GetTychoAppBuilderModelStepPredicate)
                .Select(GetTychoAppBuilderModelStepTransform);

            context.RegisterSourceOutput(
                getTychoAppBuilderModelStepResult,
                (outputContext, model) =>
                {
                    outputContext.GenerateSourceFromTemplate(
                        new AppBuilderTM(model),
                        s_appBuilderTemplate,
                        $"{model.DefinitionType.FullMetadataName}.Builder.g.cs");
                });

            return context;
        }

        private static bool GetTychoAppBuilderModelStepPredicate((TychoDefinitionKind Kind, ClassDefinitionModel Model) input)
        {
            return input.Kind == TychoDefinitionKind.App;
        }

        private static TychoAppBuilderModel GetTychoAppBuilderModelStepTransform(
            (TychoDefinitionKind Kind, ClassDefinitionModel Model) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoAppBuilderModel(input.Model.ClassType);
        }
    }
}
