using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal class TypeReference
    {
        public const string FullNamePropertyName = nameof(Type.FullName);

        public static TypeModel TypeModel { get; } = new TypeModel(typeof(Type).Namespace,nameof(Type));
    }
}
