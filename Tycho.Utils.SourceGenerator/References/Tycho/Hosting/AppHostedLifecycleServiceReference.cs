using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Hosting
{
    internal static class AppHostedLifecycleServiceReference
    {
        private const string Namespace = "Tycho.Hosting.Services";
        private const string TypeName = "AppHostedLifecycleService";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
