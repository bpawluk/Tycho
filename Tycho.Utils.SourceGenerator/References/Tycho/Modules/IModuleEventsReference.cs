using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class IModuleEventsReference
    {
        private const string Namespace = "Tycho.Modules";
        private const string TypeName = "IModuleEvents";
        private const string EventBindingTypeName = "IModuleEventBinding";

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
                    new TypeReferenceModel(Namespace, EventTypeParameterName)),
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
