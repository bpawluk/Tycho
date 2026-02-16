using System;
using System.Collections.Generic;
using System.Linq;
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
    internal static class TychoDefinitionPipeline
    {
        private static readonly string AppDefinitionTemplate = EmbeddedResource.GetContent("Templates/AppDefinition.sbncs");
        private static readonly string ModuleDefinitionTemplate = EmbeddedResource.GetContent("Templates/ModuleDefinition.sbncs");

        public static IncrementalGeneratorInitializationContext AddTychoDefinitionPipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind, ClassDefinitionModel)> pipelineBase)
        {
            var getTychoDefinitionModelStepResult = pipelineBase
                .Select(GetTychoDefinitionModelStepTransform);

            context.RegisterSourceOutput(
                getTychoDefinitionModelStepResult,
                (outputContext, model) =>
                {
                    if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    outputContext.GenerateSourceFromTemplate(
                        CreateTemplateModel(model),
                        ChooseTemplate(model),
                        $"{model.DefinitionType}.g.cs");
                });

            return context;
        }

        private static TychoDefinitionModel GetTychoDefinitionModelStepTransform((TychoDefinitionKind Kind, ClassDefinitionModel Model) input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoDefinitionModel(input.Model.ClassType, input.Kind, null); // TODO support submodules
        }

        private static object CreateTemplateModel(TychoDefinitionModel model)
        {
            return model.DefinitionKind switch
            {
                TychoDefinitionKind.App => new AppDefinitionTM(model),
                TychoDefinitionKind.Module => new ModuleDefinitionTM(model),
                _ => throw new ArgumentOutOfRangeException(nameof(model.DefinitionKind), $"Unsupported definition kind: {model.DefinitionKind}"),
            };
        }

        private static string ChooseTemplate(TychoDefinitionModel model)
        {
            return model.DefinitionKind switch
            {
                TychoDefinitionKind.App => AppDefinitionTemplate,
                TychoDefinitionKind.Module => ModuleDefinitionTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(model.DefinitionKind), $"Unsupported definition kind: {model.DefinitionKind}"),
            };
        }
    }
}
