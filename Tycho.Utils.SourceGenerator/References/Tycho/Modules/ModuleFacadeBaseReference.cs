using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class ModuleFacadeBaseReference
    {
        private const string Namespace = "Tycho.Modules.Instance";
        private const string TypeName = "ModuleFacadeBase";

        public const string ExecuteAsyncMethodName = "ExecuteAsync";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
