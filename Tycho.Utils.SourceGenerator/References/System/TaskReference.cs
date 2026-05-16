using System.Threading.Tasks;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class TaskReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(Task).Namespace, ImmutableEquatableArray<string>.Empty, nameof(Task));

        public static MethodSignatureModel ConfigureAwaitMethodSignature => new MethodSignatureModel(
            methodName: "ConfigureAwait",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                BooleanReference.TypeModel,
            }),
            result: ConfiguredValueTaskAwaitableReference.TypeModel);
    }
}
