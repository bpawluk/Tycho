using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class IAppEventsReference
    {
        private const string Namespace = "Tycho.Apps";
        private const string TypeName = "IAppEvents";
        private const string EventBindingTypeName = "IAppEventBinding";

        public static HashSet<MethodSignatureModel> HandledEventDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            HandlesWithMethodSignature,
        });

        public static HashSet<MethodSignatureModel> PublishableEventDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            ExpectsMethodSignature,
        });

        public static string EventTypeParameterName => "TEvent";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);

        public static TypeReferenceModel EventBindingTypeModel => new TypeReferenceModel(
            Namespace,
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            EventBindingTypeName,
            new ImmutableEquatableArray<TypeArgumentModel>(new[]
            {
                new TypeArgumentModel(
                    EventTypeParameterName,
                    TypeReferenceModel.TypeParameter(Namespace, EventTypeParameterName)),
            }));

        public static MethodSignatureModel ExpectsMethodSignature => new MethodSignatureModel(
            methodName: "Expects",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: EventBindingTypeModel);

        public static MethodSignatureModel HandlesWithMethodSignature => new MethodSignatureModel(
            methodName: "HandlesWith",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: TypeModel);
    }
}
