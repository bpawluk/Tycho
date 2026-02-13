using Tycho.Utils.SourceGenerator.Model.Generic;
using Tycho.Utils.SourceGenerator.Model.System;
using Tycho.Utils.SourceGenerator.References.System;

namespace Tycho.Utils.SourceGenerator.References
{
    internal static class TychoAppReference
    {
        private const string _namespace = "Tycho.Apps";
        private const string _typeName = "TychoApp";

        public static string TypeName => $"{_namespace}.{_typeName}";
        public static string GlobalTypeName => $"global::{TypeName}";
        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

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
    }
}
