using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class StringReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(string).Namespace, nameof(String));
    }
}
