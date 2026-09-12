namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class ModuleParentSymbols
    {
        public const string ParentReferenceParameter = "parentReference";
        public const string RequestDataParameter = "requestData";
        public const string CancellationTokenParameter = "cancellationToken";

        public static string GetParentInterface(string moduleClass) => $"I{moduleClass}Parent";

        public static string GetParentClass(string moduleClass) => $"{moduleClass}Parent";
    }
}
