using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Logging
{
    internal static class LoggingConfigurationReference
    {
        private const string Namespace = "Tycho.Logging";
        private const string TypeName = "LoggingConfiguration";

        public static TypeModel TypeModel => new TypeModel(Namespace,TypeName);

        public static MethodSignatureModel ConfigureLoggingMethodSignature => new MethodSignatureModel(
            methodName: "ConfigureLogging",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                ILoggingBuilderReference.TypeModel,
                IConfigurationReference.TypeModel,
            }),
            result: VoidReference.TypeModel);
    }
}
