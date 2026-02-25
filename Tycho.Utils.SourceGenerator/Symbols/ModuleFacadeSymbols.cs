namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class ModuleFacadeSymbols
    {
        public const string ModuleParameterName = "module";
        public const string RequestDataParameterName = "requestData";
        public const string CancellationTokenParameterName = "cancellationToken";

        public static string GetModuleFacadeClass(string moduleClass) => $"{moduleClass}Facade";

        public static string GetModuleFacadeInterface(string moduleClass) => $"I{moduleClass}";
    }
}
