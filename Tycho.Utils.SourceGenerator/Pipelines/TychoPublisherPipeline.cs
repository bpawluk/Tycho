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
    internal static class TychoPublisherPipeline
    {
        private static readonly string s_appPublisherTemplate = EmbeddedResource.GetContent("Templates/AppPublisher.sbncs");
        private static readonly string s_appPublisherInterfaceTemplate = EmbeddedResource.GetContent("Templates/AppPublisherInterface.sbncs");
        private static readonly string s_modulePublisherTemplate = EmbeddedResource.GetContent("Templates/ModulePublisher.sbncs");
        private static readonly string s_modulePublisherInterfaceTemplate = EmbeddedResource.GetContent("Templates/ModulePublisherInterface.sbncs");

        public static IncrementalGeneratorInitializationContext AddTychoPublisherPipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method)> pipelineBase)
        {
            IncrementalValuesProvider<(TychoDefinitionKind DefinitionKind, TypeDefinitionModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations)> getPublishableEventMethodInvocationsStepResult = pipelineBase
                .Select(GetPublishableEventMethodInvocationsStepTransform)
                .WithTrackingName("TychoPublisher.Invocations");

            IncrementalValuesProvider<TychoPublisherModel> getTychoPublisherModelStepResult = getPublishableEventMethodInvocationsStepResult
                .Select(GetTychoPublisherModelStepTransform)
                .WithTrackingName("TychoPublisher.Model");

            context.RegisterSourceOutput(
                getTychoPublisherModelStepResult,
                (outputContext, model) =>
                {
                    if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    outputContext.GenerateSourceFromTemplate(
                        CreateInterfaceTemplateModel(model),
                        ChooseInterfaceTemplate(model.DefinitionKind),
                        $"{model.DefinitionType.FullMetadataName}.Publisher.Interface.g.cs");

                    outputContext.GenerateSourceFromTemplate(
                        CreatePublisherTemplateModel(model),
                        ChoosePublisherTemplate(model.DefinitionKind),
                        $"{model.DefinitionType.FullMetadataName}.Publisher.g.cs");
                });

            return context;
        }

        private static (TychoDefinitionKind DefinitionKind, TypeDefinitionModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) GetPublishableEventMethodInvocationsStepTransform(
            (TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = input.Method.Body
                .Where(invocation => invocation.Signature.IsPublishableEventDefiningMethod())
                .ToImmutableEquatableArray();
            return (input.DefinitionKind, input.Method.ContainingType, invocations);
        }

        private static TychoPublisherModel GetTychoPublisherModelStepTransform(
            (TychoDefinitionKind DefinitionKind, TypeDefinitionModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoPublisherModel(
                input.DefinitionKind,
                input.DefinitionType,
                input.MethodInvocations
                    .Select(GetEventType)
                    .Where(eventType => eventType.HasValue)
                    .Select(eventType => eventType.Value)
                    .Distinct()
                    .ToImmutableEquatableArray());
        }

        private static TypeReferenceModel? GetEventType(MethodInvocationModel invocation)
        {
            TypeArgumentModel[] eventArguments = invocation.TypeArguments.Where(argument => argument.IsEventType()).ToArray();
            return eventArguments.Length == 1 ? eventArguments[0].Value : (TypeReferenceModel?)null;
        }

        private static object CreateInterfaceTemplateModel(TychoPublisherModel model)
        {
            return model.DefinitionKind switch
            {
                TychoDefinitionKind.App => new AppPublisherInterfaceTM(model),
                TychoDefinitionKind.Module => new ModulePublisherInterfaceTM(model),
                _ => throw new ArgumentOutOfRangeException(nameof(model.DefinitionKind), $"Unsupported definition kind: {model.DefinitionKind}"),
            };
        }

        private static object CreatePublisherTemplateModel(TychoPublisherModel model)
        {
            return model.DefinitionKind switch
            {
                TychoDefinitionKind.App => new AppPublisherTM(model),
                TychoDefinitionKind.Module => new ModulePublisherTM(model),
                _ => throw new ArgumentOutOfRangeException(nameof(model.DefinitionKind), $"Unsupported definition kind: {model.DefinitionKind}"),
            };
        }

        private static string ChooseInterfaceTemplate(TychoDefinitionKind kind)
        {
            return kind switch
            {
                TychoDefinitionKind.App => s_appPublisherInterfaceTemplate,
                TychoDefinitionKind.Module => s_modulePublisherInterfaceTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), $"Unsupported definition kind: {kind}"),
            };
        }

        private static string ChoosePublisherTemplate(TychoDefinitionKind kind)
        {
            return kind switch
            {
                TychoDefinitionKind.App => s_appPublisherTemplate,
                TychoDefinitionKind.Module => s_modulePublisherTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), $"Unsupported definition kind: {kind}"),
            };
        }
    }
}
