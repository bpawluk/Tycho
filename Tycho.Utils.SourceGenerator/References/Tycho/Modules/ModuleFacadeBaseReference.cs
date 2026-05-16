using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class ModuleFacadeBaseReference
    {
        private const string Namespace = "Tycho.Modules.Instance";
        private const string TypeName = "ModuleFacadeBase";

        public static TypeModel TypeModel => new TypeModel(Namespace, ImmutableEquatableArray<string>.Empty, TypeName);

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
