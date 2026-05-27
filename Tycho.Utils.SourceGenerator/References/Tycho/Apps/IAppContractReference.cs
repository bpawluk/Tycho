using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class IAppContractReference
    {
        private const string Namespace = "Tycho.Apps";
        private const string TypeName = "IAppContract";

        public static HashSet<MethodSignatureModel> DownstreamContractDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            ForwardsMethodSignature,
            ForwardsWithResponseMethodSignature,
            ForwardsAsMethodSignature,
            ForwardsAsWithResponseMethodSignature,
            HandlesMethodSignature,
            HandlesWithResponseMethodSignature,
        });

        public static string RequestTypeParameterName => "TRequest";
        public static string ResponseTypeParameterName => "TResponse";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);

        public static MethodSignatureModel ForwardsMethodSignature => new MethodSignatureModel(
            methodName: "Forwards",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel ForwardsWithResponseMethodSignature => new MethodSignatureModel(
            methodName: "Forwards",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel ForwardsAsMethodSignature => new MethodSignatureModel(
            methodName: "ForwardsAs",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                FuncReference.TypeModel,
            }),
            result: TypeModel);

        public static MethodSignatureModel ForwardsAsWithResponseMethodSignature => new MethodSignatureModel(
            methodName: "ForwardsAs",
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                FuncReference.TypeModel,
                FuncReference.TypeModel,
            }),
            result: TypeModel);

        public static MethodSignatureModel HandlesMethodSignature => new MethodSignatureModel(
            methodName: "Handles",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel HandlesWithResponseMethodSignature => new MethodSignatureModel(
            methodName: "Handles",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: TypeModel);
    }
}
