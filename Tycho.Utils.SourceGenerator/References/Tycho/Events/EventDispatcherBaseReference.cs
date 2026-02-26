using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class EventDispatcherBaseReference
    {
        private const string _namespace = "Tycho.Events.Handling";
        private const string _typeName = "EventDispatcherBase";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static MethodSignatureModel DispatchAsMethodSignature => new MethodSignatureModel(
            methodName: "DispatchAs",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                GuidReference.TypeModel,
                ObjectReference.TypeModel,
                IEventHandlerReference.TypeModel,
                CancellationTokenReference.TypeModel,
            }),
            result: TaskReference.TypeModel);
    }
}
