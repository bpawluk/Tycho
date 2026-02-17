using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References
{
    internal static class IRequestExecutorReference
    {
        private const string _namespace = "Tycho.Requests";
        private const string _typeName = "IRequestExecutor";

        public static string RequestTypeParameterName => "TRequest";
        public static string ResponseTypeParameterName => "TResponse";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static MethodSignatureModel ExecuteAsyncMethodSignature => new MethodSignatureModel(
            methodName: "ExecuteAsync",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                CancellationTokenReference.TypeModel,
            }),
            result: TaskReference.TypeModel);
    }
}
