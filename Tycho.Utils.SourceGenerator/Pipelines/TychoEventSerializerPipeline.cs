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
    internal static class TychoEventSerializerPipeline
    {
        private static readonly string s_appEventSerializerTemplate = EmbeddedResource.GetContent("Templates/AppEventSerializer.sbncs");
        private static readonly string s_moduleEventSerializerTemplate = EmbeddedResource.GetContent("Templates/ModuleEventSerializer.sbncs");

        public static IncrementalGeneratorInitializationContext AddTychoEventSerializerPipeline(
            this IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<(TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method)> pipelineBase)
        {
            IncrementalValuesProvider<(TychoDefinitionKind DefinitionKind, TypeDefinitionModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations)> getSerializableEventMethodInvocationsStepResult = pipelineBase
                .Select(GetSerializableEventMethodInvocationsStepTransform)
                .WithTrackingName("TychoEventSerializer.Invocations");

            IncrementalValuesProvider<TychoEventSerializerModel> getTychoEventSerializerModelStepResult = getSerializableEventMethodInvocationsStepResult
                .Select(GetTychoEventSerializerModelStepTransform)
                .WithTrackingName("TychoEventSerializer.Model");

            context.RegisterSourceOutput(
                getTychoEventSerializerModelStepResult,
                (outputContext, model) =>
                {
                    if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    outputContext.GenerateSourceFromTemplate(
                        CreateTemplateModel(model),
                        ChooseTemplate(model.DefinitionKind),
                        $"{model.DefinitionType.FullMetadataName}.EventSerializer.g.cs");
                });

            return context;
        }

        private static (TychoDefinitionKind DefinitionKind, TypeDefinitionModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) GetSerializableEventMethodInvocationsStepTransform(
            (TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = input.Method.Body
                .Where(invocation => invocation.Signature.IsHandledEventDefiningMethod())
                .ToImmutableEquatableArray();
            return (input.DefinitionKind, input.Method.ContainingType, invocations);
        }

        private static TychoEventSerializerModel GetTychoEventSerializerModelStepTransform(
            (TychoDefinitionKind DefinitionKind, TypeDefinitionModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoEventSerializerModel(
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
            if (!invocation.ReceiverType.HasValue)
            {
                return null;
            }

            TypeArgumentModel[] eventArguments = invocation.ReceiverType.Value.TypeArguments
                .Where(argument => argument.IsEventType())
                .ToArray();
            return eventArguments.Length == 1 ? eventArguments[0].Value : (TypeReferenceModel?)null;
        }

        private static object CreateTemplateModel(TychoEventSerializerModel model)
        {
            return model.DefinitionKind switch
            {
                TychoDefinitionKind.App => new AppEventSerializerTM(model),
                TychoDefinitionKind.Module => new ModuleEventSerializerTM(model),
                _ => throw new ArgumentOutOfRangeException(nameof(model.DefinitionKind), $"Unsupported definition kind: {model.DefinitionKind}"),
            };
        }

        private static string ChooseTemplate(TychoDefinitionKind kind)
        {
            return kind switch
            {
                TychoDefinitionKind.App => s_appEventSerializerTemplate,
                TychoDefinitionKind.Module => s_moduleEventSerializerTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), $"Unsupported definition kind: {kind}"),
            };
        }
    }
}
