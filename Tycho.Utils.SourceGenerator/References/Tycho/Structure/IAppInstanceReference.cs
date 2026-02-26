using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Requests;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Structure
{
    internal static class IAppInstanceReference
    {
        private const string _namespace = "Tycho.Structure";
        private const string _typeName = "IAppInstance";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static MethodSignatureModel ExecuteAsyncMethodSignature => IRequestExecutorReference.ExecuteAsyncMethodSignature;

        public static MethodSignatureModel DisposeAsyncMethodSignature => IAsyncDisposableReference.DisposeAsyncMethodSignature;
    }
}
