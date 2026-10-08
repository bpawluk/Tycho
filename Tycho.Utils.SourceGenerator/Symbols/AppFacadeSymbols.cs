namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class AppFacadeSymbols
    {
        public const string AppParameter = "app";
        public const string RequestDataParameter = "requestData";
        public const string CancellationTokenParameter = "cancellationToken";

        public static string GetAppFacadeInterface(string ownerClass) => $"I{ownerClass}";

        public static string GetAppFacadeClass(string ownerClass) => $"{ownerClass}Facade";
    }
}
