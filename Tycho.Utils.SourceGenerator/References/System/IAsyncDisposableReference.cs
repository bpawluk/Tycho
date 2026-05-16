using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class IAsyncDisposableReference
    {
        private const string Namespace = "System";
        private const string TypeName = "IAsyncDisposable";

        public static TypeModel TypeModel => new TypeModel(Namespace, ImmutableEquatableArray<string>.Empty, TypeName);

        public static MethodSignatureModel DisposeAsyncMethodSignature => new MethodSignatureModel(
            methodName: "DisposeAsync",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: ValueTaskReference.TypeModel);
    }
}
