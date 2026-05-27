using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class IHostApplicationBuilderReference
    {
        private const string Namespace = "Microsoft.Extensions.Hosting";
        private const string TypeName = "IHostApplicationBuilder";

        public const string ConfigurationPropertyName = "Configuration";
        public const string ServicesPropertyName = "Services";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
