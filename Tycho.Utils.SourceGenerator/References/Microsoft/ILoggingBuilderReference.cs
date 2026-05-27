using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class ILoggingBuilderReference
    {
        private const string Namespace = "Microsoft.Extensions.Logging";
        private const string TypeName = "ILoggingBuilder";

        public static TypeModel TypeModel => new TypeModel(Namespace, TypeName);
    }
}
