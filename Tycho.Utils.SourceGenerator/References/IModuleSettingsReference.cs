using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References
{
    internal static class IModuleSettingsReference
    {
        private const string _namespace = "Tycho.Modules";
        private const string _typeName = "IModuleSettings";

        public static string TypeName => $"{_namespace}.{_typeName}";
        public static string GlobalTypeName => $"global::{TypeName}";
        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);
    }
}