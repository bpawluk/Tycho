using System.Runtime.CompilerServices;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class ConfiguredTaskAwaitableReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(ConfiguredTaskAwaitable).Namespace, nameof(ConfiguredTaskAwaitable));
    }
}
