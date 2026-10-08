using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class IModuleContractReference
    {
        private const string Namespace = "Tycho.Modules";
        private const string TypeName = "IModuleContract";
        private const string RequestBindingTypeName = "IModuleRequestBinding";

        public static HashSet<MethodSignatureModel> DownstreamContractDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            ExpectsMethodSignature,
            ExpectsWithResponseMethodSignature,
        });

        public static HashSet<MethodSignatureModel> UpstreamContractDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            RequiresMethodSignature,
            RequiresWithResponseMethodSignature,
        });

        public static string RequestTypeParameterName => "TRequest";
        public static string ResponseTypeParameterName => "TResponse";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);

        public static TypeReferenceModel RequestBindingTypeModel => new TypeReferenceModel(
            Namespace,
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            RequestBindingTypeName,
            new ImmutableEquatableArray<TypeArgumentModel>(new[]
            {
                new TypeArgumentModel(
                    RequestTypeParameterName,
                    TypeReferenceModel.TypeParameter(Namespace, RequestTypeParameterName)),
            }));

        public static TypeReferenceModel RequestBindingWithResponseTypeModel => new TypeReferenceModel(
            Namespace,
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            RequestBindingTypeName,
            new ImmutableEquatableArray<TypeArgumentModel>(new[]
            {
                new TypeArgumentModel(
                    RequestTypeParameterName,
                    TypeReferenceModel.TypeParameter(Namespace, RequestTypeParameterName)),
                new TypeArgumentModel(
                    ResponseTypeParameterName,
                    TypeReferenceModel.TypeParameter(Namespace, ResponseTypeParameterName)),
            }));

        public static MethodSignatureModel ExpectsMethodSignature => new MethodSignatureModel(
            methodName: "Expects",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: RequestBindingTypeModel);

        public static MethodSignatureModel ExpectsWithResponseMethodSignature => new MethodSignatureModel(
            methodName: "Expects",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: RequestBindingWithResponseTypeModel);

        public static MethodSignatureModel RequiresMethodSignature => new MethodSignatureModel(
            methodName: "Requires",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel RequiresWithResponseMethodSignature => new MethodSignatureModel(
            methodName: "Requires",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: TypeModel);
    }
}
