using System.Threading.Tasks;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class TaskReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(Task).Namespace, nameof(Task));

        public static MethodSignatureModel ConfigureAwaitMethodSignature => new MethodSignatureModel(
            methodName: "ConfigureAwait",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                BooleanReference.TypeModel,
            }),
            result: ConfiguredValueTaskAwaitableReference.TypeModel);
    }
}
