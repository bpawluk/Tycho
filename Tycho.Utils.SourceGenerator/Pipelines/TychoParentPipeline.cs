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
            IncrementalValuesProvider<(TychoDefinitionKind, ClassDefinitionModel)> pipelineBase)
        {
            IncrementalValuesProvider<MethodDefinitionModel> getDefineContractMethodDefinitionsStepResult = pipelineBase
                .Where(GetDefineContractMethodDefinitionsStepPredicate)
                .Select(GetDefineContractMethodDefinitionsStepTransform);

            IncrementalValuesProvider<(TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations)> getRequirementMethodInvocationsStepResult = getDefineContractMethodDefinitionsStepResult
                .Select(GetRequirementMethodInvocationsStepTransform);

            IncrementalValuesProvider<TychoParentModel> getTychoParentModelStepResult = getRequirementMethodInvocationsStepResult
                .Select(GetTychoParentModelStepTransform);

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

        private static bool GetDefineContractMethodDefinitionsStepPredicate((TychoDefinitionKind Kind, ClassDefinitionModel Model) input)
        {
            return input.Kind == TychoDefinitionKind.Module;
        }

        private static MethodDefinitionModel GetDefineContractMethodDefinitionsStepTransform(
            (TychoDefinitionKind DefinitionKind, ClassDefinitionModel Model) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return input.Model.Methods.Single(method => method.Signature.IsDefineContractMethod());
        }

        private static (TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) GetRequirementMethodInvocationsStepTransform(
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
            (TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoParentModel(
                input.DefinitionType,
                input.MethodInvocations
                    .Select(GetTychoRequestModel)
                    .Distinct()
                    .ToImmutableEquatableArray());
        }

        private static TychoRequestModel GetTychoRequestModel(MethodInvocationModel model)
        {
            TypeModel requestType = model.TypeArguments.Single(argument => argument.IsRequestType()).Value;
            if (model.TypeArguments.Any(argument => argument.IsResponseType()))
            {
                TypeModel responseType = model.TypeArguments.Single(argument => argument.IsResponseType()).Value;
                return new TychoRequestModel(requestType, responseType);
            }
            return new TychoRequestModel(requestType);
        }
    }
}
