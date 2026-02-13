using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Model.Partial;
using Tycho.Utils.SourceGenerator.References.System;

namespace Tycho.Utils.SourceGenerator.References
{
    internal static class IAppStructureReference
    {
        private const string _namespace = "Tycho.Apps";
        private const string _typeName = "IAppStructure";

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
                ActionReference.OneParamTypeModel,
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
                ActionReference.OneParamTypeModel,
                IModuleSettingsReference.TypeModel,
            }),
            result: TypeModel);
    }
}
