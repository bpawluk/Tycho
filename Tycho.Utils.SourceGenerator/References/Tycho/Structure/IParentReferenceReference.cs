using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Structure
{
    internal static class IParentReferenceReference
    {
        private const string Namespace = "Tycho.Structure.Parent";
        private const string TypeName = "IParentReference";

        public static TypeModel TypeModel => new TypeModel(Namespace,TypeName);
    }
}
