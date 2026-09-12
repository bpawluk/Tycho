using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class TychoModuleReference
    {
        private const string Namespace = "Tycho.Modules";
        private const string TypeName = "TychoModule";

        public static string FullName => $"{Namespace}.{TypeName}";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);

        public static MethodSignatureModel DefineContractMethodSignature => new MethodSignatureModel(
            methodName: "DefineContract",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                IModuleContractReference.TypeModel,
            }),
            result: VoidReference.TypeModel);

        public static MethodSignatureModel DefineEventsMethodSignature => new MethodSignatureModel(
            methodName: "DefineEvents",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                IModuleEventsReference.TypeModel,
            }),
            result: VoidReference.TypeModel);

        public static MethodSignatureModel IncludeModulesMethodSignature => new MethodSignatureModel(
            methodName: "IncludeModules",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                IModuleStructureReference.TypeModel,
            }),
            result: VoidReference.TypeModel);
    }
}
