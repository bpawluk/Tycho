using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class IModuleStructureReference
    {
        private const string Namespace = "Tycho.Modules";
        private const string TypeName = "IModuleStructure";

        public static HashSet<MethodSignatureModel> SubmoduleDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            UsesMethodSignature,
            UsesWithContractFulfillmentMethodSignature,
            UsesWithSettingsMethodSignature,
            UsesWithContractFulfillmentAndSettingsMethodSignature,
        });

        public static string ModuleTypeParameterName => "TModule";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);

        public static MethodSignatureModel UsesMethodSignature => new MethodSignatureModel(
            methodName: "Uses",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel UsesWithContractFulfillmentMethodSignature => new MethodSignatureModel(
            methodName: "Uses",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                ActionReference.TypeModel,
            }),
            result: TypeModel);

        public static MethodSignatureModel UsesWithSettingsMethodSignature => new MethodSignatureModel(
            methodName: "Uses",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                IModuleSettingsReference.TypeModel,
            }),
            result: TypeModel);

        public static MethodSignatureModel UsesWithContractFulfillmentAndSettingsMethodSignature => new MethodSignatureModel(
            methodName: "Uses",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                ActionReference.TypeModel,
                IModuleSettingsReference.TypeModel,
            }),
            result: TypeModel);
    }
}
