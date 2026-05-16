using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class ServiceProviderServiceExtensionsReference
    {
        private const string Namespace = "Microsoft.Extensions.DependencyInjection";
        private const string TypeName = "ServiceProviderServiceExtensions";

        public static TypeModel TypeModel => new TypeModel(Namespace, ImmutableEquatableArray<string>.Empty, TypeName);

        public static MethodSignatureModel GetRequiredServiceMethodSignature => new MethodSignatureModel(
            methodName: "GetRequiredService",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IServiceCollectionReference.TypeModel,
            }),
            result: IServiceCollectionReference.TypeModel);
    }
}
