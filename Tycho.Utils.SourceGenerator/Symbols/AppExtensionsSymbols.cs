namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class AppExtensionsSymbols
    {
        public const string AppParameter = "app";
        public const string AppDefinitionParameter = "appDefinition";
        public const string AppBuilderBaseParameter = "appBuilderBase";
        public const string AppBuilderVariable = "appBuilder";
        public const string BuilderParameter = "builder";
        public const string ServiceDescriptorParameter = "descriptor";
        public const string ServiceProviderParameter = "provider";

        public const string CreateAppBuilderMethod = "CreateAppBuilder";

        public static string GetAddAppMethod(string appClass) => $"Add{appClass}";

        public static string GetAppSetupExtensionsClass(string appClass) => $"{appClass}SetupExtensions";
    }
}
