using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class PublisherBaseReference
    {
        private const string Namespace = "Tycho.Events.Publishing";
        private const string TypeName = "PublisherBase";

        public static TypeModel TypeModel => new TypeModel(Namespace, ImmutableEquatableArray<string>.Empty, TypeName);

        public static MethodSignatureModel PublishAsyncMethodSignature => new MethodSignatureModel(
            methodName: "PublishAsync",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                ObjectReference.TypeModel,
                CancellationTokenReference.TypeModel,
            }),
            result: TaskReference.TypeModel);
    }
}
