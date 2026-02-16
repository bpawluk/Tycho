using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References
{
    internal static class IModuleStructureReference
    {
        private const string _namespace = "Tycho.Modules";
        private const string _typeName = "IModuleStructure";

        public static HashSet<MethodSignatureModel> SubmoduleDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            UsesMethodSignature,
            UsesWithContractFulfillmentMethodSignature,
            UsesWithSettingsMethodSignature,
            UsesWithContractFulfillmentAndSettingsMethodSignature,
        });

        public static string TypeName => $"{_namespace}.{_typeName}";
        public static string GlobalTypeName => $"global::{TypeName}";
        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static MethodSignatureModel UsesMethodSignature => new MethodSignatureModel(
            methodName: "Uses",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel UsesWithContractFulfillmentMethodSignature => new MethodSignatureModel(
            methodName: "Uses",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                ActionReference.TypeModel,
            }),
            result: TypeModel);

        public static MethodSignatureModel UsesWithSettingsMethodSignature => new MethodSignatureModel(
            methodName: "Uses",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                IModuleSettingsReference.TypeModel,
            }),
            result: TypeModel);

        public static MethodSignatureModel UsesWithContractFulfillmentAndSettingsMethodSignature => new MethodSignatureModel(
            methodName: "Uses",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                ActionReference.TypeModel,
                IModuleSettingsReference.TypeModel,
            }),
            result: TypeModel);
    }
}
