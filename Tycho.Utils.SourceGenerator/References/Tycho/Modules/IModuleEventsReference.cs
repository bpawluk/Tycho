using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class IModuleEventsReference
    {
        private const string _namespace = "Tycho.Modules";
        private const string _typeName = "IModuleEvents";
        private const string _eventRoutingTypeName = "IEventRouting";

        public static HashSet<MethodSignatureModel> EventDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            HandlesMethodSignature,
        });

        public static HashSet<MethodSignatureModel> PublishableEventDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            HandlesMethodSignature,
            RoutesMethodSignature,
        });

        public static string EventTypeParameterName => "TEvent";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static TypeModel EventRoutingTypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _eventRoutingTypeName);

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
