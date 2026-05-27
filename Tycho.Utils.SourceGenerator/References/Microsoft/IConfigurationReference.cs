using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class IConfigurationReference
    {
        private const string Namespace = "Microsoft.Extensions.Configuration";
        private const string TypeName = "IConfiguration";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
