using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class TychoAppReference
    {
        private const string Namespace = "Tycho.Apps";
        private const string TypeName = "TychoApp";

        public const string CreateAppBuilderBaseMethodName = "CreateAppBuilderBase";

        public static string FullName => $"{Namespace}.{TypeName}";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);

        public static MethodSignatureModel DefineContractMethodSignature => new MethodSignatureModel(
            methodName: "DefineContract",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                IAppContractReference.TypeModel,
            }),
            result: VoidReference.TypeModel);

        public static MethodSignatureModel DefineEventsMethodSignature => new MethodSignatureModel(
            methodName: "DefineEvents",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                IAppEventsReference.TypeModel,
            }),
            result: VoidReference.TypeModel);

        public static MethodSignatureModel IncludeModulesMethodSignature => new MethodSignatureModel(
            methodName: "IncludeModules",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                IAppStructureReference.TypeModel,
            }),
            result: VoidReference.TypeModel);
    }
}
