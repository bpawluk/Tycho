namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class ModuleDefinitionSymbols
    {
        public const string SetupMethod = "Setup";
        public const string ModuleParameter = "module";
        public const string ProviderParameter = "provider";

        public static string GetSetupClass(string ownerClass, string ownerTypeParametersSuffix = null) => $"{ownerClass}Setup{ownerTypeParametersSuffix ?? string.Empty}";
    }
}
