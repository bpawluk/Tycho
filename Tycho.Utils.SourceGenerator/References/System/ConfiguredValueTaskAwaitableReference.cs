using System.Runtime.CompilerServices;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class ConfiguredValueTaskAwaitableReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(ConfiguredValueTaskAwaitable).Namespace, nameof(ConfiguredValueTaskAwaitable));
    }
}
