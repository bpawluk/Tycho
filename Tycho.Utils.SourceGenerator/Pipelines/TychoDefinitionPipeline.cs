using System;
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
            var getIncludeModulesMethodDefinitionStepResult = pipelineBase
                .Select(GetIncludeModulesMethodDefinitionStepTransform);

            var getSubmoduleMethodInvocationsStepResult = getIncludeModulesMethodDefinitionStepResult
                .Select(GetSubmoduleMethodInvocationsStepTransform);

            var getTychoDefinitionModelStepResult = getSubmoduleMethodInvocationsStepResult
                .Select(GetTychoDefinitionModelStepTransform);

            context.RegisterSourceOutput(
                getTychoDefinitionModelStepResult,
                (outputContext, model) =>
                {
                    if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    outputContext.GenerateSourceFromTemplate(
                        CreateTemplateModel(model),
                        ChooseTemplate(model),
                        $"{model.DefinitionType}.Setup.g.cs");
                });

            return context;
        }

        private static (TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method) GetIncludeModulesMethodDefinitionStepTransform(
            (TychoDefinitionKind DefinitionKind, ClassDefinitionModel Model) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return (input.DefinitionKind, input.Model.Methods.FirstOrDefault(method => method.Signature.IsIncludeModulesMethod()));
        }

        private static (TychoDefinitionKind DefinitionKind, TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) GetSubmoduleMethodInvocationsStepTransform(
            (TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = input.Method.Body
                .Where(invocation => invocation.Signature.IsSubmoduleDefiningMethod())
                .ToImmutableEquatableArray();
            return (input.DefinitionKind, input.Method.ContainingType, invocations);
        }

        private static TychoDefinitionModel GetTychoDefinitionModelStepTransform(
            (TychoDefinitionKind DefinitionKind, TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoDefinitionModel(
                input.DefinitionKind,
                input.DefinitionType,
                input.MethodInvocations
                    .Select(invocation => invocation.TypeArguments
                        .Single(argument => argument.IsModuleType())
                        .Value)
                    .Distinct()
                    .ToImmutableEquatableArray());
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
