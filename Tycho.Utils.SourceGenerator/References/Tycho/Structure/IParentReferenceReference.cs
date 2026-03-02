using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Structure
{
    internal static class IParentReferenceReference
    {
        private const string _namespace = "Tycho.Structure.Parent";
        private const string _typeName = "IParentReference";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);
    }
}
