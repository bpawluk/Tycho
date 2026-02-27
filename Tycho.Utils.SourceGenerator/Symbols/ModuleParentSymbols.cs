namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class ModuleParentSymbols
    {
        public const string ParentReferenceParameter = "parentReference";
        public const string RequestDataParameter = "requestData";
        public const string CancellationTokenParameter = "cancellationToken";

        public static string GetModuleParentClass(string moduleClass) => $"{moduleClass}Parent";

        public static string GetModuleParentInterface(string moduleClass) => $"I{moduleClass}Parent";
    }
}
