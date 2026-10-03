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
    internal static class TychoParentPipeline
    {
        private static readonly string s_moduleParentTemplate = EmbeddedResource.GetContent("Templates/ModuleParent.sbncs");
        private static readonly string s_moduleParentInterfaceTemplate = EmbeddedResource.GetContent("Templates/ModuleParentInterface.sbncs");

        public static IncrementalGeneratorInitializationContext AddTychoParentPipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method)> pipelineBase)
        {
            IncrementalValuesProvider<MethodDefinitionModel> getDefineContractMethodDefinitionsStepResult = pipelineBase
                .Where(GetDefineContractMethodDefinitionsStepPredicate)
                .Select(GetDefineContractMethodDefinitionsStepTransform)
                .WithTrackingName("TychoParent.Definition");

            IncrementalValuesProvider<(TypeDefinitionModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations)> getRequirementMethodInvocationsStepResult = getDefineContractMethodDefinitionsStepResult
                .Select(GetRequirementMethodInvocationsStepTransform)
                .WithTrackingName("TychoParent.Invocations");

            IncrementalValuesProvider<TychoParentModel> getTychoParentModelStepResult = getRequirementMethodInvocationsStepResult
                .Select(GetTychoParentModelStepTransform)
                .WithTrackingName("TychoParent.Model");

            context.RegisterSourceOutput(
                getTychoParentModelStepResult,
                (outputContext, model) =>
                {
                    outputContext.GenerateSourceFromTemplate(
                        new ModuleParentInterfaceTM(model),
                        s_moduleParentInterfaceTemplate,
                        $"{model.DefinitionType.FullMetadataName}.Parent.Interface.g.cs");

                    outputContext.GenerateSourceFromTemplate(
                        new ModuleParentTM(model),
                        s_moduleParentTemplate,
                        $"{model.DefinitionType.FullMetadataName}.Parent.g.cs");
                });

            return context;
        }

        private static bool GetDefineContractMethodDefinitionsStepPredicate(
            (TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method) input)
        {
            return input.DefinitionKind == TychoDefinitionKind.Module;
        }

        private static MethodDefinitionModel GetDefineContractMethodDefinitionsStepTransform(
            (TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return input.Method;
        }

        private static (TypeDefinitionModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) GetRequirementMethodInvocationsStepTransform(
            MethodDefinitionModel input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = input.Body
                .Where(invocation => invocation.Signature.IsUpstreamContractDefiningMethod())
                .ToImmutableEquatableArray();
            return (input.ContainingType, invocations);
        }

        private static TychoParentModel GetTychoParentModelStepTransform(
            (TypeDefinitionModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoParentModel(
                input.DefinitionType,
                input.MethodInvocations
                    .Select(GetTychoRequestModel)
                    .Where(request => request.HasValue)
                    .Select(request => request.Value)
                    .Distinct()
                    .ToImmutableEquatableArray());
        }

        private static TychoRequestModel? GetTychoRequestModel(MethodInvocationModel model)
        {
            TypeArgumentModel[] requestArguments = model.TypeArguments.Where(argument => argument.IsRequestType()).ToArray();
            TypeArgumentModel[] responseArguments = model.TypeArguments.Where(argument => argument.IsResponseType()).ToArray();
            if (requestArguments.Length != 1 || responseArguments.Length > 1)
            {
                return null;
            }

            return responseArguments.Length == 1
                ? new TychoRequestModel(requestArguments[0].Value, responseArguments[0].Value)
                : new TychoRequestModel(requestArguments[0].Value);
        }
    }
}
