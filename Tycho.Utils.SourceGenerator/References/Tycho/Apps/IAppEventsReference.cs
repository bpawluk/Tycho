using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class IAppEventsReference
    {
        private const string Namespace = "Tycho.Apps";
        private const string TypeName = "IAppEvents";
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

        public static TypeModel TypeModel => new TypeModel(Namespace, TypeName);

        public static TypeModel EventRoutingTypeModel => new TypeModel(Namespace, EventRoutingTypeName);

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
