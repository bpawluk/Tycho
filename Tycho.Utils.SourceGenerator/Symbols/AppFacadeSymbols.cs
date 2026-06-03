namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class AppFacadeSymbols
    {
        public const string AppParameter = "app";
        public const string RequestDataParameter = "requestData";
        public const string CancellationTokenParameter = "cancellationToken";

        public static string GetAppFacadeInterface(string ownerClass, string ownerTypeParametersSuffix = null) => $"I{ownerClass}{ownerTypeParametersSuffix ?? string.Empty}";

        public static string GetAppFacadeClass(string ownerClass, string ownerTypeParametersSuffix = null) => $"{ownerClass}Facade{ownerTypeParametersSuffix ?? string.Empty}";
    }
}
