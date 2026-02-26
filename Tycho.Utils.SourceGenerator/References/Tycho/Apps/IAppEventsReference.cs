using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class IAppEventsReference
    {
        private const string _namespace = "Tycho.Apps";
        private const string _typeName = "IAppEvents";

        public static HashSet<MethodSignatureModel> EventDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            HandlesMethodSignature,
        });

        public static string EventTypeParameterName => "TEvent";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static MethodSignatureModel HandlesMethodSignature => new MethodSignatureModel(
            methodName: "Handles",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: TypeModel);
    }
}
