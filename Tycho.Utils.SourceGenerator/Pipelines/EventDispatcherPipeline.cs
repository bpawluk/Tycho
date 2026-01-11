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
    internal static class EventDispatcherPipeline
    {
        private static readonly string EventDispatcherTemplate = EmbeddedResource.GetContent("Templates/EventDispatcher.sbncs");

        private static readonly MethodSignatureModel DefineAppEventsMethodSignature = new MethodSignatureModel(
            methodName: "DefineEvents",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                new TypeModel("Tycho.Apps", ImmutableEquatableArray<string>.Empty, "IAppEvents"),
            }),
            result: new TypeModel("System", ImmutableEquatableArray<string>.Empty, "Void"));

        private static readonly MethodSignatureModel DefineModuleEventsMethodSignature = new MethodSignatureModel(
            methodName: "DefineEvents",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                new TypeModel("Tycho.Modules", ImmutableEquatableArray<string>.Empty, "IModuleEvents"),
            }),
            result: new TypeModel("System", ImmutableEquatableArray<string>.Empty, "Void"));

        private static readonly MethodSignatureModel AppHandlesMethodSignature = new MethodSignatureModel(
            methodName: "Handles",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: new TypeModel("Tycho.Apps", ImmutableEquatableArray<string>.Empty, "IAppEvents"));

        private static readonly MethodSignatureModel ModuleHandlesMethodSignature = new MethodSignatureModel(
            methodName: "Handles",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: new TypeModel("Tycho.Modules", ImmutableEquatableArray<string>.Empty, "IModuleEvents"));

        public static IncrementalGeneratorInitializationContext AddEventDispatcherPipeline(
            this IncrementalGeneratorInitializationContext context, 
            IncrementalValuesProvider<(TychoDefinitionKind, ClassDefinitionModel)> pipelineBase)
        {
            var getDefineEventsMethodDefinitionsStepResult = pipelineBase
                .SelectMany(GetDefineEventsMethodDefinitionsStepTransform);

            var getHandlesMethodInvocationsStepResult = getDefineEventsMethodDefinitionsStepResult
                .Select(GetHandlesMethodInvocationsStepTransform);

            var getEventDispatcherModelStepResult = getHandlesMethodInvocationsStepResult
                .Select(GetEventDispatcherModelStepTransform);

            context.RegisterSourceOutput(
                getEventDispatcherModelStepResult,
                (outputContext, model) =>
                {
                    outputContext.GenerateSourceFromTemplate(
                        model,
                        EventDispatcherTemplate,
                        $"{model.DefinitionType}.EventDispatcher.g.cs");
                });

            return context;
        }

        private static ImmutableEquatableArray<MethodDefinitionModel> GetDefineEventsMethodDefinitionsStepTransform((TychoDefinitionKind, ClassDefinitionModel Model) input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return input.Model.Methods
                .Where(method =>
                    method.Signature == DefineAppEventsMethodSignature ||
                    method.Signature == DefineModuleEventsMethodSignature)
                .ToImmutableEquatableArray();
        }

        private static (TypeModel, ImmutableEquatableArray<MethodInvocationModel>) GetHandlesMethodInvocationsStepTransform(MethodDefinitionModel model, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var invocations = model.Body
                .Where(invocation =>
                    invocation.Signature == AppHandlesMethodSignature ||
                    invocation.Signature == ModuleHandlesMethodSignature)
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
                    .Select(methodInvocationModel => methodInvocationModel.TypeParameters.First().ParameterValue)
                    .ToImmutableEquatableArray());
        }
    }
}
