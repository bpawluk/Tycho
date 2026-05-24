using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class IConfigurationReference
    {
        private const string Namespace = "Microsoft.Extensions.Configuration";
        private const string TypeName = "IConfiguration";

        public static TypeModel TypeModel => new TypeModel(Namespace,TypeName);
    }
}
