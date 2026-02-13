using Tycho.Utils.SourceGenerator.Model.Partial;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class VoidReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(
            typeof(void).Namespace, 
            ImmutableEquatableArray<string>.Empty, 
            typeof(void).Name);
    }
}
