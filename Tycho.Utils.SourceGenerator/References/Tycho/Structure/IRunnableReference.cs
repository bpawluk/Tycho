using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Structure
{
    internal static class IRunnableReference
    {
        private const string Namespace = "Tycho.Structure";
        private const string TypeName = "IRunnable";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
