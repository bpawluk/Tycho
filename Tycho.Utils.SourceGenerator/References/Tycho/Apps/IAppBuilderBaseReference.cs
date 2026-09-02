using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class IAppBuilderBaseReference
    {
        private const string Namespace = "Tycho.Apps";
        private const string TypeName = "IAppBuilderBase";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);

        public static MethodSignatureModel BuildMethodSignature => new MethodSignatureModel(
            methodName: "Build",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                IServiceProviderReference.TypeModel,
            }),
            result: IAppReference.TypeModel);
    }
}
