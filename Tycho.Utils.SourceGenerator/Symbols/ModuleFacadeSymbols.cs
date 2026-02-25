namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class ModuleFacadeSymbols
    {
        public const string ModuleParameter = "module";
        public const string RequestDataParameter = "requestData";
        public const string CancellationTokenParameter = "cancellationToken";

        public static string GetModuleFacadeClass(string moduleClass) => $"{moduleClass}Facade";

        public static string GetModuleFacadeInterface(string moduleClass) => $"I{moduleClass}";
    }
}
