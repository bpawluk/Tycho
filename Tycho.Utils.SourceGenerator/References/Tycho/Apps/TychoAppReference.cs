using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class TychoAppReference
    {
        private const string Namespace = "Tycho.Apps";
        private const string TypeName = "TychoApp";

        public static string FullName => $"{Namespace}.{TypeName}";

        public static TypeModel TypeModel => new TypeModel(Namespace,TypeName);

        public static MethodSignatureModel DefineContractMethodSignature => new MethodSignatureModel(
            methodName: "DefineContract",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IAppContractReference.TypeModel,
            }),
            result: VoidReference.TypeModel);

        public static MethodSignatureModel DefineEventsMethodSignature => new MethodSignatureModel(
            methodName: "DefineEvents",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IAppEventsReference.TypeModel,
            }),
            result: VoidReference.TypeModel);

        public static MethodSignatureModel IncludeModulesMethodSignature => new MethodSignatureModel(
            methodName: "IncludeModules",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IAppStructureReference.TypeModel,
            }),
            result: VoidReference.TypeModel);

        public static MethodSignatureModel WithConfigurationBaseMethodSignature => new MethodSignatureModel(
            methodName: "WithConfigurationBase",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IConfigurationReference.TypeModel,
            }),
            result: VoidReference.TypeModel);

        public static MethodSignatureModel WithLoggingBaseMethodSignature => new MethodSignatureModel(
            methodName: "WithLoggingBase",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                ActionReference.TypeModel,
            }),
            result: VoidReference.TypeModel);

        public static MethodSignatureModel RunBaseAsyncMethodSignature => new MethodSignatureModel(
            methodName: "RunBaseAsync",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: TaskReference.TypeModel);

        public static MethodSignatureModel AutoSetupMethodSignature => new MethodSignatureModel(
            methodName: "__AutoSetup__",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IServiceCollectionReference.TypeModel,
            }),
            result: VoidReference.TypeModel);
    }
}
