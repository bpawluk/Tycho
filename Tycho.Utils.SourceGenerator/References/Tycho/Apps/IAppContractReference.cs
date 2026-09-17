using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class IAppContractReference
    {
        private const string Namespace = "Tycho.Apps";
        private const string TypeName = "IAppContract";
        private const string RequestBindingTypeName = "IAppRequestBinding";

        public static HashSet<MethodSignatureModel> DownstreamContractDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            ExpectsMethodSignature,
            ExpectsWithResponseMethodSignature,
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
    }
}
