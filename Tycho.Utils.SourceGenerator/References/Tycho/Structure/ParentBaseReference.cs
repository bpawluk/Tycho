using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Structure
{
    internal static class ParentBaseReference
    {
        private const string Namespace = "Tycho.Structure.Parent";
        private const string TypeName = "ParentBase";

        public static TypeModel TypeModel => new TypeModel(Namespace, TypeName);

        public static MethodSignatureModel ExecuteAsyncMethodSignature => new MethodSignatureModel(
            methodName: "ExecuteAsync",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                ObjectReference.TypeModel,
                CancellationTokenReference.TypeModel,
            }),
            result: TaskReference.TypeModel);
    }
}
