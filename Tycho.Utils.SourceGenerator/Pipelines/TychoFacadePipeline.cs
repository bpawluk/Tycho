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
    internal static class TychoFacadePipeline
    {
        private static readonly string AppFacadeTemplate = EmbeddedResource.GetContent("Templates/AppFacade.sbncs");
        private static readonly string AppInterfaceTemplate = EmbeddedResource.GetContent("Templates/AppInterface.sbncs");
        private static readonly string ModuleFacadeTemplate = EmbeddedResource.GetContent("Templates/ModuleFacade.sbncs");
        private static readonly string ModuleInterfaceTemplate = EmbeddedResource.GetContent("Templates/ModuleInterface.sbncs");

        public static IncrementalGeneratorInitializationContext AddTychoFacadePipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind, ClassDefinitionModel)> pipelineBase)
        {
            var getDefineContractMethodDefinitionsStepResult = pipelineBase
                .Select(GetDefineContractMethodDefinitionsStepTransform);

            var getContractMethodInvocationsStepResult = getDefineContractMethodDefinitionsStepResult
                .Select(GetContractMethodInvocationsStepTransform);

            var getTychoFacadeModelStepResult = getContractMethodInvocationsStepResult
                .Select(GetTychoFacadeModelStepTransform);

            context.RegisterSourceOutput(
                getTychoFacadeModelStepResult,
                (outputContext, model) =>
                {
                    if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    var templateModel = CreateTemplateModel(model);

                    outputContext.GenerateSourceFromTemplate(
                        templateModel,
                        ChooseInterfaceTemplate(model.DefinitionKind),
                        $"{model.DefinitionType}.Interface.g.cs");

                    outputContext.GenerateSourceFromTemplate(
                        templateModel,
                        ChooseFacadeTemplate(model.DefinitionKind),
                        $"{model.DefinitionType}.Facade.g.cs");
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

        private static (TychoDefinitionKind DefinitionKind, TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) GetContractMethodInvocationsStepTransform(
            (TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = input.Method.Body
                .Where(invocation => invocation.Signature.IsContractDefiningMethod)
                .ToImmutableEquatableArray();
            return (input.DefinitionKind, input.Method.ContainingType, invocations);
        }

        private static TychoFacadeModel GetTychoFacadeModelStepTransform(
            (TychoDefinitionKind DefinitionKind, TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoFacadeModel(
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

        private static object CreateTemplateModel(TychoFacadeModel model)
        {
            return model.DefinitionKind switch
            {
                TychoDefinitionKind.App => new AppFacadeTM(model),
                TychoDefinitionKind.Module => new ModuleFacadeTM(model),
                _ => throw new ArgumentOutOfRangeException(nameof(model.DefinitionKind), $"Unsupported definition kind: {model.DefinitionKind}"),
            };
        }

        private static string ChooseInterfaceTemplate(TychoDefinitionKind kind)
        {
            return kind switch
            {
                TychoDefinitionKind.App => AppInterfaceTemplate,
                TychoDefinitionKind.Module => ModuleInterfaceTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), $"Unsupported definition kind: {kind}"),
            };
        }

        private static string ChooseFacadeTemplate(TychoDefinitionKind kind)
        {
            return kind switch
            {
                TychoDefinitionKind.App => AppFacadeTemplate,
                TychoDefinitionKind.Module => ModuleFacadeTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), $"Unsupported definition kind: {kind}"),
            };
        }
    }
}
