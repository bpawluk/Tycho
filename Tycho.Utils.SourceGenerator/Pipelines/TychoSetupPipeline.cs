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
    internal static class TychoSetupPipeline
    {
        private static readonly string s_appSetupTemplate = EmbeddedResource.GetContent("Templates/AppSetup.sbncs");
        private static readonly string s_moduleSetupTemplate = EmbeddedResource.GetContent("Templates/ModuleSetup.sbncs");

        public static IncrementalGeneratorInitializationContext AddTychoSetupPipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind SetupKind, MethodDefinitionModel Method)> pipelineBase)
        {
            IncrementalValuesProvider<(TychoDefinitionKind SetupKind, TypeDefinitionModel SetupType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations)> getSubmoduleMethodInvocationsStepResult = pipelineBase
                .Select(GetSubmoduleMethodInvocationsStepTransform)
                .WithTrackingName("TychoSetup.Invocations");

            IncrementalValuesProvider<TychoSetupModel> getTychoSetupModelStepResult = getSubmoduleMethodInvocationsStepResult
                .Select(GetTychoSetupModelStepTransform)
                .WithTrackingName("TychoSetup.Model");

            context.RegisterSourceOutput(
                getTychoSetupModelStepResult,
                (outputContext, model) =>
                {
                    if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    outputContext.GenerateSourceFromTemplate(
                        CreateTemplateModel(model),
                        ChooseTemplate(model),
                        $"{model.DefinitionType.FullMetadataName}.Setup.g.cs");
                });

            return context;
        }

        private static (TychoDefinitionKind SetupKind, TypeDefinitionModel SetupType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) GetSubmoduleMethodInvocationsStepTransform(
            (TychoDefinitionKind SetupKind, MethodDefinitionModel Method) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = input.Method.Body
                .Where(invocation => invocation.Signature.IsSubmoduleDefiningMethod())
                .ToImmutableEquatableArray();
            return (input.SetupKind, input.Method.ContainingType, invocations);
        }

        private static TychoSetupModel GetTychoSetupModelStepTransform(
            (TychoDefinitionKind SetupKind, TypeDefinitionModel SetupType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoSetupModel(
                input.SetupKind,
                input.SetupType,
                input.MethodInvocations
                    .Select(GetModuleType)
                    .Where(moduleType => moduleType.HasValue)
                    .Select(moduleType => moduleType.Value)
                    .Distinct()
                    .ToImmutableEquatableArray());
        }

        private static TypeReferenceModel? GetModuleType(MethodInvocationModel invocation)
        {
            TypeArgumentModel[] moduleArguments = invocation.TypeArguments.Where(argument => argument.IsModuleType()).ToArray();
            return moduleArguments.Length == 1 ? moduleArguments[0].Value : (TypeReferenceModel?)null;
        }

        private static object CreateTemplateModel(TychoSetupModel model)
        {
            return model.DefinitionKind switch
            {
                TychoDefinitionKind.App => new AppSetupTM(model),
                TychoDefinitionKind.Module => new ModuleSetupTM(model),
                _ => throw new ArgumentOutOfRangeException(nameof(model.DefinitionKind), $"Unsupported definition kind: {model.DefinitionKind}"),
            };
        }

        private static string ChooseTemplate(TychoSetupModel model)
        {
            return model.DefinitionKind switch
            {
                TychoDefinitionKind.App => s_appSetupTemplate,
                TychoDefinitionKind.Module => s_moduleSetupTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(model.DefinitionKind), $"Unsupported definition kind: {model.DefinitionKind}"),
            };
        }
    }
}
