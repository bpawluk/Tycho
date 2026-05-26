using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class IAppReference
    {
        private const string Namespace = "Tycho.Apps.Instance";
        private const string TypeName = "IApp";

        public static TypeModel TypeModel => new TypeModel(Namespace, TypeName);
    }
}
