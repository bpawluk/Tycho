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
        private static readonly string ModuleParentTemplate = EmbeddedResource.GetContent("Templates/ModuleParent.sbncs");
        private static readonly string ModuleParentInterfaceTemplate = EmbeddedResource.GetContent("Templates/ModuleParentInterface.sbncs");

        public static IncrementalGeneratorInitializationContext AddTychoParentPipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind, ClassDefinitionModel)> pipelineBase)
        {
            var getDefineContractMethodDefinitionsStepResult = pipelineBase
                .Where(input => input.Item1 == TychoDefinitionKind.Module)
                .Select(GetDefineContractMethodDefinitionsStepTransform);

            var getRequirementMethodInvocationsStepResult = getDefineContractMethodDefinitionsStepResult
                .Select(GetRequirementMethodInvocationsStepTransform);

            var getTychoParentModelStepResult = getRequirementMethodInvocationsStepResult
                .Select(GetTychoParentModelStepTransform);

            context.RegisterSourceOutput(
                getTychoParentModelStepResult,
                (outputContext, model) =>
                {
                    if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    outputContext.GenerateSourceFromTemplate(
                        new ModuleParentInterfaceTM(model),
                        ModuleParentInterfaceTemplate,
                        $"{model.DefinitionType}.Parent.Interface.g.cs");

                    outputContext.GenerateSourceFromTemplate(
                        new ModuleParentTM(model),
                        ModuleParentTemplate,
                        $"{model.DefinitionType}.Parent.g.cs");
                });

            return context;
        }

        private static (TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method) GetDefineContractMethodDefinitionsStepTransform(
            (TychoDefinitionKind DefinitionKind, ClassDefinitionModel Model) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return (input.DefinitionKind, input.Model.Methods.Single(method => method.Signature.IsDefineContractMethod));
        }

        private static (TychoDefinitionKind DefinitionKind, TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) GetRequirementMethodInvocationsStepTransform(
            (TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = input.Method.Body
                .Where(invocation => invocation.Signature.IsRequiredContractDefiningMethod)
                .ToImmutableEquatableArray();
            return (input.DefinitionKind, input.Method.ContainingType, invocations);
        }

        private static TychoParentModel GetTychoParentModelStepTransform(
            (TychoDefinitionKind DefinitionKind, TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoParentModel(
                input.DefinitionType,
                input.DefinitionKind,
                input.MethodInvocations
                    .Select(GetTychoRequestModel)
                    .Distinct()
                    .ToImmutableEquatableArray());
        }

        private static TychoRequestModel GetTychoRequestModel(MethodInvocationModel model)
        {
            var requestType = model.TypeArguments.Single(argument => argument.IsRequestType).Value;
            if (model.TypeArguments.Any(argument => argument.IsResponseType))
            {
                var responseType = model.TypeArguments.Single(argument => argument.IsResponseType).Value;
                return new TychoRequestModel(requestType, responseType);
            }
            return new TychoRequestModel(requestType);
        }
    }
}
