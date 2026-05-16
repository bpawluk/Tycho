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

        public static TypeModel TypeModel => new TypeModel(Namespace, ImmutableEquatableArray<string>.Empty, TypeName);

        public static TypeModel EventRoutingTypeModel => new TypeModel(Namespace, ImmutableEquatableArray<string>.Empty, EventRoutingTypeName);

        public static MethodSignatureModel HandlesMethodSignature => new MethodSignatureModel(
            methodName: "Handles",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: TypeModel);

        public static MethodSignatureModel RoutesMethodSignature => new MethodSignatureModel(
            methodName: "Routes",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: EventRoutingTypeModel);
    }
}
