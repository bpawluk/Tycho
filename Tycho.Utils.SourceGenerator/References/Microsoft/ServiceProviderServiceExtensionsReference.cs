using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class ServiceProviderServiceExtensionsReference
    {
        private const string _namespace = "Microsoft.Extensions.DependencyInjection";
        private const string _typeName = "ServiceProviderServiceExtensions";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static MethodSignatureModel GetRequiredServiceMethodSignature => new MethodSignatureModel(
            methodName: "GetRequiredService",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IServiceCollectionReference.TypeModel,
            }),
            result: IServiceCollectionReference.TypeModel);
    }
}
