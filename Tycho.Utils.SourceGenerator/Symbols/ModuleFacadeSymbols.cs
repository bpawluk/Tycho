namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class ModuleFacadeSymbols
    {
        public const string ModuleParameter = "module";
        public const string RequestDataParameter = "requestData";
        public const string CancellationTokenParameter = "cancellationToken";

        public static string GetModuleFacadeInterface(string ownerClass, string ownerTypeParametersSuffix = null) => $"I{ownerClass}{ownerTypeParametersSuffix ?? string.Empty}";

        public static string GetModuleFacadeClass(string ownerClass, string ownerTypeParametersSuffix = null) => $"{ownerClass}Facade{ownerTypeParametersSuffix ?? string.Empty}";
    }
}
