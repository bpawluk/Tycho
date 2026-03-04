namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class AppExtensionsSymbols
    {
        public const string AppParameter = "app";
        public const string BuilderParameter = "builder";
        public const string LoggingParameter = "logging";

        public static string GetAddAppMethod(string appClass) => $"Add{appClass}";

        public static string GetAppSetupExtensionsClass(string appClass) => $"{appClass}SetupExtensions";

    }
}
