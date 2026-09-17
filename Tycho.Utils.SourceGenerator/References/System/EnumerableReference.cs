using System.Linq;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class EnumerableReference
    {
        public const string AnyMethodName = nameof(Enumerable.Any);

        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(
            typeof(Enumerable).Namespace,
            nameof(Enumerable));
    }
}
