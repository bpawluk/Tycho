using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References
{
    internal static class IModuleContractReference
    {
        private const string _namespace = "Tycho.Modules";
        private const string _typeName = "IModuleContract";

        public static HashSet<MethodSignatureModel> ContractDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            ForwardsMethodSignature,
            ForwardsWithResponseMethodSignature,
            ForwardsAsMethodSignature,
            ForwardsAsWithResponseMethodSignature,
            HandlesMethodSignature,
            HandlesWithResponseMethodSignature,
        });

        public static HashSet<MethodSignatureModel> RequirementDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            RequiresMethodSignature,
            RequiresWithResponseMethodSignature,
        });

        public static string RequestTypeParameterName => "TRequest";
        public static string ResponseTypeParameterName => "TResponse";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static MethodSignatureModel ForwardsMethodSignature => new MethodSignatureModel(
            methodName: "Forwards",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel ForwardsWithResponseMethodSignature => new MethodSignatureModel(
            methodName: "Forwards",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel ForwardsAsMethodSignature => new MethodSignatureModel(
            methodName: "ForwardsAs",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                FuncReference.TypeModel,
            }),
            result: TypeModel);

        public static MethodSignatureModel ForwardsAsWithResponseMethodSignature => new MethodSignatureModel(
            methodName: "ForwardsAs",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                FuncReference.TypeModel,
                FuncReference.TypeModel,
            }),
            result: TypeModel);

        public static MethodSignatureModel HandlesMethodSignature => new MethodSignatureModel(
            methodName: "Handles",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel HandlesWithResponseMethodSignature => new MethodSignatureModel(
            methodName: "Handles",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel RequiresMethodSignature => new MethodSignatureModel(
            methodName: "Requires",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel RequiresWithResponseMethodSignature => new MethodSignatureModel(
            methodName: "Requires",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: TypeModel);
    }
}
