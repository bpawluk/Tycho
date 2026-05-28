namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class ModuleParentSymbols
    {
        public const string ParentInterface = "IParent";
        public const string ParentReferenceParameter = "parentReference";
        public const string RequestDataParameter = "requestData";
        public const string CancellationTokenParameter = "cancellationToken";

        public static string GetParentClass(string moduleClass, string moduleTypeParametersSuffix = null) => $"{moduleClass}Parent{moduleTypeParametersSuffix ?? string.Empty}";
    }
}
