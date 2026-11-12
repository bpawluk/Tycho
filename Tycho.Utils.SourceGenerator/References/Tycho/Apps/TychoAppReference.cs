namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class TychoAppReference
    {
        private const string Namespace = "Tycho.Apps";
        private const string TypeName = "TychoApp";

        public const string CreateAppBuilderBaseMethodName = "CreateAppBuilderBase";

        public static string FullName => $"{Namespace}.{TypeName}";
    }
}
