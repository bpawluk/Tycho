using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class IModuleContractReference
    {
        private const string Namespace = "Tycho.Modules";
        private const string TypeName = "IModuleContract";
        private const string RequestExpectationTypeName = "IModuleRequestExpectation";

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

        public static TypeReferenceModel RequestExpectationTypeModel => new TypeReferenceModel(
            Namespace,
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            RequestExpectationTypeName,
            new ImmutableEquatableArray<TypeArgumentModel>(new[]
            {
                new TypeArgumentModel(
                    RequestTypeParameterName,
                    new TypeReferenceModel(Namespace, RequestTypeParameterName)),
            }));

        public static TypeReferenceModel RequestExpectationWithResponseTypeModel => new TypeReferenceModel(
            Namespace,
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            RequestExpectationTypeName,
            new ImmutableEquatableArray<TypeArgumentModel>(new[]
            {
                new TypeArgumentModel(
                    RequestTypeParameterName,
                    new TypeReferenceModel(Namespace, RequestTypeParameterName)),
                new TypeArgumentModel(
                    ResponseTypeParameterName,
                    new TypeReferenceModel(Namespace, ResponseTypeParameterName)),
            }));

        public static MethodSignatureModel ExpectsMethodSignature => new MethodSignatureModel(
            methodName: "Expects",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: RequestExpectationTypeModel);

        public static MethodSignatureModel ExpectsWithResponseMethodSignature => new MethodSignatureModel(
            methodName: "Expects",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: RequestExpectationWithResponseTypeModel);

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
