using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Extensions;
using Tycho.Utils.SourceGenerator.Model;
using Tycho.Utils.SourceGenerator.Model.Partial;
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
                        model,
                        ChooseTemplate(model.DefinitionKind),
                        $"{model.DefinitionType}.g.cs");
                });

            return context;
        }

        private static TychoDefinitionModel GetTychoDefinitionModelStepTransform((TychoDefinitionKind Kind, ClassDefinitionModel Model) input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoDefinitionModel(input.Model.ClassType, input.Kind);
        }

        private static string ChooseTemplate(TychoDefinitionKind kind)
        {
            return kind switch
            {
                TychoDefinitionKind.App => AppDefinitionTemplate,
                TychoDefinitionKind.Module => ModuleDefinitionTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), $"Unsupported definition kind: {kind}"),
            };
        }
    }
}
