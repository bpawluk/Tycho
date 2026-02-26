using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class IAppReference
    {
        private const string _namespace = "Tycho.Apps.Instance";
        private const string _typeName = "IApp";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);
    }
}
