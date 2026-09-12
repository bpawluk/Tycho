namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class AppBuilderSymbols
    {
        public const string AppParameter = "app";
        public const string AppBuilderBaseField = "_appBuilderBase";
        public const string AppBuilderBaseParameter = "appBuilderBase";
        public const string ParentServiceProviderParameter = "parentServiceProvider";

        public const string BuildMethod = "Build";

        public static string GetAppBuilderClass(string appClass) => $"{appClass}Builder";
    }
}
