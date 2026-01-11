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
    internal static class TychoFacadePipeline
    {
        public const string TychoRequestTypeParameterName = "TRequest";
        public const string TychoResponseTypeParameterName = "TResponse";

        private static readonly string AppFacadeTemplate = EmbeddedResource.GetContent("Templates/AppFacade.sbncs");
        //private static readonly string ModuleFacadeTemplate = EmbeddedResource.GetContent("Templates/ModuleFacade.sbncs");

        private static readonly TypeModel IAppContractType = new TypeModel(
            "Tycho.Apps",
            ImmutableEquatableArray<string>.Empty,
            "IAppContract");

        private static readonly TypeModel IModuleContractType = new TypeModel(
            "Tycho.Modules",
            ImmutableEquatableArray<string>.Empty,
            "IModuleContract");

        private static readonly MethodSignatureModel DefineAppContractMethodSignature = new MethodSignatureModel(
            methodName: "DefineContract",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                new TypeModel("Tycho.Apps", ImmutableEquatableArray<string>.Empty, "IAppContract"),
            }),
            result: new TypeModel("System", ImmutableEquatableArray<string>.Empty, "Void"));

        private static readonly MethodSignatureModel DefineModuleContractMethodSignature = new MethodSignatureModel(
            methodName: "DefineContract",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                new TypeModel("Tycho.Modules", ImmutableEquatableArray<string>.Empty, "IModuleContract"),
            }),
            result: new TypeModel("System", ImmutableEquatableArray<string>.Empty, "Void"));

        public static IncrementalGeneratorInitializationContext AddTychoFacadePipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind, ClassDefinitionModel)> pipelineBase)
        {
            var getDefineContractMethodDefinitionsStepResult = pipelineBase
                .SelectMany(GetDefineContractMethodDefinitionsStepTransform);

            var getIAppContractMethodInvocationsStepResult = getDefineContractMethodDefinitionsStepResult
                .Select(GetIAppContractMethodInvocationsStepTransform);

            var getTychoFacadeModelStepResult = getIAppContractMethodInvocationsStepResult
                .Select(GetTychoFacadeModelStepTransform);

            context.RegisterSourceOutput(
                getTychoFacadeModelStepResult,
                (outputContext, model) =>
                {
                    //if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    outputContext.GenerateSourceFromTemplate(
                        model,
                        AppFacadeTemplate,
                        $"{model.DefinitionType}.Facade.g.cs");
                });

            return context;
        }

        private static ImmutableEquatableArray<MethodDefinitionModel> GetDefineContractMethodDefinitionsStepTransform((TychoDefinitionKind, ClassDefinitionModel Model) input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return input.Model.Methods
                .Where(method =>
                    method.Signature == DefineAppContractMethodSignature ||
                    method.Signature == DefineModuleContractMethodSignature)
                .ToImmutableEquatableArray();
        }

        private static (TypeModel, ImmutableEquatableArray<MethodInvocationModel>) GetIAppContractMethodInvocationsStepTransform(MethodDefinitionModel model, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = model.Body
                .Where(invocation => invocation.ReceiverType == IAppContractType)
                .ToImmutableEquatableArray();
            return (model.ContainingType, invocations);
        }

        private static TychoFacadeModel GetTychoFacadeModelStepTransform(
            (TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocation) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoFacadeModel(
                input.DefinitionType,
                input.MethodInvocation
                    .Where(methodInvocationModel => methodInvocationModel.TypeParameters.Any(parameter => parameter.ParameterName == TychoRequestTypeParameterName))
                    .Select(GetTychoRequestModel)
                    .ToImmutableEquatableArray()
                );
        }

        private static TychoRequestModel GetTychoRequestModel(MethodInvocationModel model)
        {
            var requestType = model.TypeParameters.Single(parameter => parameter.ParameterName == TychoRequestTypeParameterName).ParameterValue;
            if (model.TypeParameters.Any(parameter => parameter.ParameterName == TychoResponseTypeParameterName))
            {
                return new TychoRequestModel(
                    requestType,
                    model.TypeParameters.First(parameter => parameter.ParameterName == TychoResponseTypeParameterName).ParameterValue);
            }
            return new TychoRequestModel(requestType);
        }
    }
}
