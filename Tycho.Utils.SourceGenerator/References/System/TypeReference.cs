using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal class TypeReference
    {
        public const string FullNamePropertyName = nameof(Type.FullName);

        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(Type).Namespace, nameof(Type));
    }
}
