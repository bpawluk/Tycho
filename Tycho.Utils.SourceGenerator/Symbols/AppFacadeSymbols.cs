namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class AppFacadeSymbols
    {
        public const string AppParameter = "app";
        public const string RequestDataParameter = "requestData";
        public const string CancellationTokenParameter = "cancellationToken";

        public static string GetAppFacadeClass(string appClass, string appTypeParametersSuffix = null) => $"{appClass}Facade{appTypeParametersSuffix ?? string.Empty}";

        public static string GetAppFacadeInterface(string appClass) => $"I{appClass}";
    }
}
