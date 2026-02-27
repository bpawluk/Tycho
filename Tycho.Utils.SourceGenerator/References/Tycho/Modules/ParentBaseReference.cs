using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class ParentBaseReference
    {
        private const string _namespace = "Tycho.Structure.External";
        private const string _typeName = "ParentBase";

        public static string RequestTypeParameterName => "TRequest";
        public static string ResponseTypeParameterName => "TResponse";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

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
