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
    internal static class TychoSetupPipeline
    {
        private static readonly string AppSetupTemplate = EmbeddedResource.GetContent("Templates/AppSetup.sbncs");

        public static IncrementalGeneratorInitializationContext AddTychoSetupPipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind, ClassDefinitionModel)> pipelineBase)
        {
            var getTychoSetupModelStepResult = pipelineBase
                .Where(GetTychoSetupModelStepPredicate)
                .Select(GetTychoSetupModelStepTransform);

            context.RegisterSourceOutput(
                getTychoSetupModelStepResult,
                (outputContext, model) =>
                {
                    outputContext.GenerateSourceFromTemplate(
                        new AppSetupTM(model),
                        AppSetupTemplate,
                        $"{model.DefinitionType}.Setup.g.cs");
                });

            return context;
        }

        private static bool GetTychoSetupModelStepPredicate((TychoDefinitionKind Kind, ClassDefinitionModel Model) input)
        {
            return input.Kind == TychoDefinitionKind.App;
        }

        private static TychoSetupModel GetTychoSetupModelStepTransform((TychoDefinitionKind Kind, ClassDefinitionModel Model) input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoSetupModel(input.Model.ClassType);
        }
    }
}
