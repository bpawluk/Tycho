using System.Collections.Generic;
using Tycho.Utils.SourceGenerator.Model.Generic;
using Tycho.Utils.SourceGenerator.Model.System;

namespace Tycho.Utils.SourceGenerator.References
{
    internal static class IAppEventsReference
    {
        private const string _namespace = "Tycho.Apps";
        private const string _typeName = "IAppEvents";

        public static HashSet<MethodSignatureModel> EventDefiningMethods { get; } = new HashSet<MethodSignatureModel>(new[]
        {
            HandlesMethodSignature,
        });

        public static string TypeName => $"{_namespace}.{_typeName}";
        public static string GlobalTypeName => $"global::{TypeName}";
        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static string EventTypeParameterName => "TEvent";

        public static MethodSignatureModel HandlesMethodSignature => new MethodSignatureModel(
            methodName: "Handles",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: TypeModel);
    }
}
