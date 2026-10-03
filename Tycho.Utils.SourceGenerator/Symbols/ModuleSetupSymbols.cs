namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class ModuleSetupSymbols
    {
        public const string SetupMethod = "Setup";
        public const string ModuleParameter = "module";

        public static string GetSetupClass(string ownerClass) => $"{ownerClass}Setup";
    }
}
