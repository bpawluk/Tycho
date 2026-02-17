using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class IHostApplicationBuilderReference
    {
        private const string _namespace = "Microsoft.Extensions.Hosting";
        private const string _typeName = "IHostApplicationBuilder";

        public const string ConfigurationProperty = "Configuration";
        public const string ServicesProperty = "Services";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);
    }
}
