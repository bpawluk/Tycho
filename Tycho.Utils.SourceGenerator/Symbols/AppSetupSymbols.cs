namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class AppSetupSymbols
    {
        public const string SetupMethod = "Setup";
        public const string WithConfigurationMethod = "WithConfiguration";
        public const string WithLoggingMethod = "WithLogging";
        public const string RunAsyncMethod = "RunAsync";
        public const string GlobalConfigurationParameter = "globalConfiguration";
        public const string LoggingSetupParameter = "loggingSetup";
        public const string AppParameter = "app";

        public static string GetSetupClass(string ownerClass, string ownerTypeParametersSuffix = null) => $"{ownerClass}Setup{ownerTypeParametersSuffix ?? string.Empty}";
    }
}
