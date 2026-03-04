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
        private static readonly string AppExtensionsTemplate = EmbeddedResource.GetContent("Templates/AppExtensions.sbncs");

        public static IncrementalGeneratorInitializationContext AddTychoExtensionsPipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind, ClassDefinitionModel)> pipelineBase)
        {
            var getTychoExtensionsModelStepResult = pipelineBase
                .Where(GetTychoExtensionsModelStepPredicate)
                .Select(GetTychoExtensionsModelStepTransform);

            context.RegisterSourceOutput(
                getTychoExtensionsModelStepResult,
                (outputContext, model) =>
                {
                    outputContext.GenerateSourceFromTemplate(
                        new AppExtensionsTM(model),
                        AppExtensionsTemplate,
                        $"{model.DefinitionType}.Extensions.g.cs");
                });

            return context;
        }

        private static bool GetTychoExtensionsModelStepPredicate((TychoDefinitionKind Kind, ClassDefinitionModel Model) input)
        {
            return input.Kind == TychoDefinitionKind.App;
        }

        private static TychoExtensionsModel GetTychoExtensionsModelStepTransform((TychoDefinitionKind Kind, ClassDefinitionModel Model) input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoExtensionsModel(input.Model.ClassType);
        }
    }
}
