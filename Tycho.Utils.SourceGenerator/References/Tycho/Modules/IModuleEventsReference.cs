using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class IModuleEventsReference
    {
        private const string Namespace = "Tycho.Modules";
        private const string TypeName = "IModuleEvents";
        private const string EventRoutingTypeName = "IEventRouting";

        public static HashSet<MethodSignatureModel> HandledEventDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            HandlesMethodSignature,
        });

        public static HashSet<MethodSignatureModel> HandledOrRoutedEventDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            HandlesMethodSignature,
            RoutesMethodSignature,
        });

        public static string EventTypeParameterName => "TEvent";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);

        public static TypeReferenceModel EventRoutingTypeModel => new TypeReferenceModel(Namespace, EventRoutingTypeName);

        public static MethodSignatureModel HandlesMethodSignature => new MethodSignatureModel(
            methodName: "Handles",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel RoutesMethodSignature => new MethodSignatureModel(
            methodName: "Routes",
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: EventRoutingTypeModel);
    }
}
