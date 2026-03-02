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
    internal static class EventDispatcherPipeline
    {
        private static readonly string EventDispatcherTemplate = EmbeddedResource.GetContent("Templates/EventDispatcher.sbncs");

        public static IncrementalGeneratorInitializationContext AddEventDispatcherPipeline(
            this IncrementalGeneratorInitializationContext context, 
            IncrementalValuesProvider<(TychoDefinitionKind, ClassDefinitionModel)> pipelineBase)
        {
            var getDefineEventsMethodDefinitionStepResult = pipelineBase
                .Select(GetDefineEventsMethodDefinitionStepTransform);

            var getHandlesMethodInvocationsStepResult = getDefineEventsMethodDefinitionStepResult
                .Select(GetHandlesMethodInvocationsStepTransform);

            var getEventDispatcherModelStepResult = getHandlesMethodInvocationsStepResult
                .Select(GetEventDispatcherModelStepTransform);

            context.RegisterSourceOutput(
                getEventDispatcherModelStepResult,
                (outputContext, model) =>
                {
                    outputContext.GenerateSourceFromTemplate(
                        new EventDispatcherTM(model),
                        EventDispatcherTemplate,
                        $"{model.DefinitionType}.Dispatcher.g.cs");
                });

            return context;
        }

        private static MethodDefinitionModel GetDefineEventsMethodDefinitionStepTransform((TychoDefinitionKind, ClassDefinitionModel Model) input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return input.Model.Methods.Single(method => method.Signature.IsDefineEventsMethod);
        }

        private static (TypeModel, ImmutableEquatableArray<MethodInvocationModel>) GetHandlesMethodInvocationsStepTransform(MethodDefinitionModel model, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = model.Body
                .Where(invocation => invocation.Signature.IsEventDefiningMethod)
                .ToImmutableEquatableArray();
            return (model.ContainingType, invocations);
        }

        private static EventDispatcherModel GetEventDispatcherModelStepTransform(
            (TypeModel DefinitionType, ImmutableEquatableArray<MethodInvocationModel> MethodInvocation) input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return new EventDispatcherModel(
                input.DefinitionType,
                input.MethodInvocation
                    .Select(methodInvocationModel => methodInvocationModel.TypeArguments
                        .Single(argument => argument.IsEventType())
                        .Value)
                    .ToImmutableEquatableArray());
        }
    }
}
