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
            IncrementalValuesProvider<(TychoDefinitionKind, ClassDefinitionModel)> pipelineBase)
        {
            IncrementalValuesProvider<(TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method)> getDefineEventsMethodDefinitionsStepResult = pipelineBase
                .Select(GetDefineEventsMethodDefinitionsStepTransform);

            IncrementalValuesProvider<(TychoDefinitionKind DefinitionKind, TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations)> getPublishableEventMethodInvocationsStepResult = getDefineEventsMethodDefinitionsStepResult
                .Select(GetPublishableEventMethodInvocationsStepTransform);

            IncrementalValuesProvider<TychoPublisherModel> getTychoPublisherModelStepResult = getPublishableEventMethodInvocationsStepResult
                .Select(GetTychoPublisherModelStepTransform);

            context.RegisterSourceOutput(
                getTychoPublisherModelStepResult,
                (outputContext, model) =>
                {
                    if (model.DefinitionKind == TychoDefinitionKind.Unknown) return;

                    outputContext.GenerateSourceFromTemplate(
                        CreateInterfaceTemplateModel(model),
                        ChooseInterfaceTemplate(model.DefinitionKind),
                        $"{model.DefinitionType.HintName}.Publisher.Interface.g.cs");

                    outputContext.GenerateSourceFromTemplate(
                        CreatePublisherTemplateModel(model),
                        ChoosePublisherTemplate(model.DefinitionKind),
                        $"{model.DefinitionType.HintName}.Publisher.g.cs");
                });

            return context;
        }

        private static (TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method) GetDefineEventsMethodDefinitionsStepTransform(
            (TychoDefinitionKind DefinitionKind, ClassDefinitionModel Model) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return (input.DefinitionKind, input.Model.Methods.Single(method => method.Signature.IsDefineEventsMethod()));
        }

        private static (TychoDefinitionKind DefinitionKind, TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) GetPublishableEventMethodInvocationsStepTransform(
            (TychoDefinitionKind DefinitionKind, MethodDefinitionModel Method) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = input.Method.Body
                .Where(invocation => invocation.Signature.IsHandledOrRoutedEventDefiningMethod())
                .ToImmutableEquatableArray();
            return (input.DefinitionKind, input.Method.ContainingType, invocations);
        }

        private static TychoPublisherModel GetTychoPublisherModelStepTransform(
            (TychoDefinitionKind DefinitionKind, TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocations) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new TychoPublisherModel(
                input.DefinitionKind,
                input.DefinitionType,
                input.MethodInvocations
                    .Select(invocation => invocation.TypeArguments
                        .Single(argument => argument.IsEventType())
                        .Value)
                    .Distinct()
                    .ToImmutableEquatableArray());
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
