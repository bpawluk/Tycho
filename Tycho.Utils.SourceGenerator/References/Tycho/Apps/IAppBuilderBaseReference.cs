using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class IAppBuilderBaseReference
    {
        private const string Namespace = "Tycho.Apps";
        private const string TypeName = "IAppBuilderBase";

        public const string BuildMethodName = "Build";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
