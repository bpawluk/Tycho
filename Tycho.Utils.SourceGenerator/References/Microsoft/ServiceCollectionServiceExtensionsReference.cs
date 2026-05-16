using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class ServiceCollectionServiceExtensionsReference
    {
        private const string Namespace = "Microsoft.Extensions.DependencyInjection";
        private const string TypeName = "ServiceCollectionServiceExtensions";

        public static TypeModel TypeModel => new TypeModel(Namespace, ImmutableEquatableArray<string>.Empty, TypeName);

        public static MethodSignatureModel AddSingletonMethodSignature => new MethodSignatureModel(
            methodName: "AddSingleton",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IServiceCollectionReference.TypeModel,
            }),
            result: IServiceCollectionReference.TypeModel);

        public static MethodSignatureModel AddTransientMethodSignature => new MethodSignatureModel(
            methodName: "AddTransient",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IServiceCollectionReference.TypeModel,
            }),
            result: IServiceCollectionReference.TypeModel);
    }
}
