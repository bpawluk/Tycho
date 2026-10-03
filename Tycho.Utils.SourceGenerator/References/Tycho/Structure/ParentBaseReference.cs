using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Structure
{
    internal static class ParentBaseReference
    {
        private const string Namespace = "Tycho.Structure.Parent";
        private const string TypeName = "ParentBase";

        public const string ExecuteAsyncMethodName = "ExecuteAsync";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
