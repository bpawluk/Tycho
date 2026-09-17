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
    internal static class TychoFacadePipeline
    {
        private static readonly string s_appFacadeTemplate = EmbeddedResource.GetContent("Templates/AppFacade.sbncs");
        private static readonly string s_appInterfaceTemplate = EmbeddedResource.GetContent("Templates/AppFacadeInterface.sbncs");
        private static readonly string s_moduleFacadeTemplate = EmbeddedResource.GetContent("Templates/ModuleFacade.sbncs");
        private static readonly string s_moduleInterfaceTemplate = EmbeddedResource.GetContent("Templates/ModuleFacadeInterface.sbncs");

        public static IncrementalGeneratorInitializationContext AddTychoFacadePipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method)> pipelineBase)
        {
            IncrementalValuesProvider<(TychoDefinitionKind DefinitionKind, TypeDefinitionModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations)> getContractMethodInvocationsStepResult = pipelineBase
                .Select(GetContractMethodInvocationsStepTransform)
                .WithTrackingName("TychoFacade.Invocations");

            IncrementalValuesProvider<TychoFacadeModel> getTychoFacadeModelStepResult = getContractMethodInvocationsStepResult
                .Select(GetTychoFacadeModelStepTransform)
                .WithTrackingName("TychoFacade.Model");

            context.RegisterSourceOutput(
                getTychoFacadeModelStepResult,
                (outputContext, model) =>
                {
                    if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    outputContext.GenerateSourceFromTemplate(
                        CreateInterfaceTemplateModel(model),
                        ChooseInterfaceTemplate(model.DefinitionKind),
                        $"{model.DefinitionType.FullMetadataName}.Facade.Interface.g.cs");

                    outputContext.GenerateSourceFromTemplate(
                        CreateFacadeTemplateModel(model),
                        ChooseFacadeTemplate(model.DefinitionKind),
                        $"{model.DefinitionType.FullMetadataName}.Facade.g.cs");
                });

            return context;
        }

        private static (TychoDefinitionKind DefinitionKind, TypeDefinitionModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) GetContractMethodInvocationsStepTransform(
            (TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = input.Method.Body
                .Where(invocation => invocation.Signature.IsDownstreamContractDefiningMethod())
                .ToImmutableEquatableArray();
            return (input.DefinitionKind, input.Method.ContainingType, invocations);
        }

        private static TychoFacadeModel GetTychoFacadeModelStepTransform(
            (TychoDefinitionKind DefinitionKind, TypeDefinitionModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoFacadeModel(
                input.DefinitionKind,
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

        private static object CreateInterfaceTemplateModel(TychoFacadeModel model)
        {
            return model.DefinitionKind switch
            {
                TychoDefinitionKind.App => new AppFacadeInterfaceTM(model),
                TychoDefinitionKind.Module => new ModuleFacadeInterfaceTM(model),
                _ => throw new ArgumentOutOfRangeException(nameof(model.DefinitionKind), $"Unsupported definition kind: {model.DefinitionKind}"),
            };
        }

        private static object CreateFacadeTemplateModel(TychoFacadeModel model)
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
                TychoDefinitionKind.App => s_appInterfaceTemplate,
                TychoDefinitionKind.Module => s_moduleInterfaceTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), $"Unsupported definition kind: {kind}"),
            };
        }

        private static string ChooseFacadeTemplate(TychoDefinitionKind kind)
        {
            return kind switch
            {
                TychoDefinitionKind.App => s_appFacadeTemplate,
                TychoDefinitionKind.Module => s_moduleFacadeTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), $"Unsupported definition kind: {kind}"),
            };
        }
    }
}
