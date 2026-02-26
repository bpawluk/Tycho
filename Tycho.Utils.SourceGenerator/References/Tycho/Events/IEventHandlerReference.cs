using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class IEventHandlerReference
    {
        private const string _namespace = "Tycho.Events";
        private const string _typeName = "IEventHandler";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static MethodSignatureModel HandleAsyncMethodSignature => new MethodSignatureModel(
            methodName: "HandleAsync",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                EventContextReference.TypeModel,
                CancellationTokenReference.TypeModel,
            }),
            result: TaskReference.TypeModel);
    }
}
