using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Model.Partial;
using Tycho.Utils.SourceGenerator.References.System;

namespace Tycho.Utils.SourceGenerator.References
{
    internal static class IAppContractReference
    {
        private const string _namespace = "Tycho.Apps";
        private const string _typeName = "IAppContract";

        public static HashSet<MethodSignatureModel> ContractDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            ForwardsMethodSignature,
            ForwardsWithResponseMethodSignature,
            ForwardsAsMethodSignature,
            ForwardsAsWithResponseMethodSignature,
            HandlesMethodSignature,
            HandlesWithResponseMethodSignature,
        });

        public static string TypeName => $"{_namespace}.{_typeName}";
        public static string GlobalTypeName => $"global::{TypeName}";
        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static string RequestTypeParameterName => "TRequest";
        public static string ResponseTypeParameterName => "TResponse";

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
    }
}
