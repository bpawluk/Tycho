using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class ServiceCollectionHostedServiceExtensionsReference
    {
        private const string Namespace = "Microsoft.Extensions.DependencyInjection";
        private const string TypeName = "ServiceCollectionHostedServiceExtensions";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);

        public static MethodSignatureModel AddHostedServiceMethodSignature => new MethodSignatureModel(
            methodName: "AddHostedService",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                IServiceCollectionReference.TypeModel,
            }),
            result: IServiceCollectionReference.TypeModel);
    }
}
