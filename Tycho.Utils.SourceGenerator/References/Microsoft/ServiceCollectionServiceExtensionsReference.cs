using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class ServiceCollectionServiceExtensionsReference
    {
        private const string Namespace = "Microsoft.Extensions.DependencyInjection";
        private const string TypeName = "ServiceCollectionServiceExtensions";

        public const string AddSingletonMethodName = "AddSingleton";
        public const string AddTransientMethodName = "AddTransient";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
