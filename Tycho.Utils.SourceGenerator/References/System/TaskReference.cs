using System.Threading.Tasks;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class TaskReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(Task).Namespace, nameof(Task));
    }
}
