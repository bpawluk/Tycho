using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class AppFacadeBaseReference
    {
        private const string Namespace = "Tycho.Apps.Instance";
        private const string TypeName = "AppFacadeBase";

        public const string ExecuteAsyncMethodName = "ExecuteAsync";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
