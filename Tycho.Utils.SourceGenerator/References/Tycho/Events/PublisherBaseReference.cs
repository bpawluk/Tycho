using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class PublisherBaseReference
    {
        private const string Namespace = "Tycho.Events.Publishing";
        private const string TypeName = "PublisherBase";

        public const string PublishAsyncMethodName = "PublishAsync";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
