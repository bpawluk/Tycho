using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class IModuleSettingsReference
    {
        private const string Namespace = "Tycho.Modules";
        private const string TypeName = "IModuleSettings";

        public static TypeModel TypeModel => new TypeModel(Namespace, ImmutableEquatableArray<string>.Empty, TypeName);
    }
}
