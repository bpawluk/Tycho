using System.Threading.Tasks;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class ValueTaskReference
    {
        public static TypeReferenceModel TypeModel => new TypeReferenceModel(typeof(ValueTask).Namespace, nameof(ValueTask));

        public static MethodSignatureModel ConfigureAwaitMethodSignature => new MethodSignatureModel(
            methodName: "ConfigureAwait",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                BooleanReference.TypeModel,
            }),
            result: ConfiguredValueTaskAwaitableReference.TypeModel);
    }
}
