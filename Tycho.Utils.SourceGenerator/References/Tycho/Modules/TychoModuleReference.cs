using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class TychoModuleReference
    {
        private const string Namespace = "Tycho.Modules";
        private const string TypeName = "TychoModule";

        public static string FullName => $"{Namespace}.{TypeName}";

        public static TypeModel TypeModel => new TypeModel(Namespace, TypeName);

        public static MethodSignatureModel DefineContractMethodSignature => new MethodSignatureModel(
            methodName: "DefineContract",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IModuleContractReference.TypeModel,
            }),
            result: VoidReference.TypeModel);

        public static MethodSignatureModel DefineEventsMethodSignature => new MethodSignatureModel(
            methodName: "DefineEvents",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IModuleEventsReference.TypeModel,
            }),
            result: VoidReference.TypeModel);

        public static MethodSignatureModel IncludeModulesMethodSignature => new MethodSignatureModel(
            methodName: "IncludeModules",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IModuleStructureReference.TypeModel,
            }),
            result: VoidReference.TypeModel);

        public static MethodSignatureModel AutoSetupMethodSignature => new MethodSignatureModel(
            methodName: "__AutoSetup__",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IServiceCollectionReference.TypeModel,
            }),
            result: VoidReference.TypeModel);
    }
}
