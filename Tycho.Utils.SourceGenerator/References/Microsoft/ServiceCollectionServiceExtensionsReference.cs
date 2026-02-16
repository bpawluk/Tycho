using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class ServiceCollectionServiceExtensionsReference
    {
        private const string _namespace = "Microsoft.Extensions.DependencyInjection";
        private const string _typeName = "ServiceCollectionServiceExtensions";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static MethodSignatureModel AddTransientMethodSignature => new MethodSignatureModel(
            methodName: "AddTransient",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IServiceCollectionReference.TypeModel,
            }),
            result: IServiceCollectionReference.TypeModel);
    }
}
