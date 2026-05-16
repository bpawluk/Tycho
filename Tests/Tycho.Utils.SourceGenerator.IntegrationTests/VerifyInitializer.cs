using System.Runtime.CompilerServices;

namespace Tycho.Utils.SourceGenerator.IntegrationTests;

internal static class VerifyInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        DerivePathInfo((sourceFile, projectDirectory, type, method) =>
        {
            string directory = Path.Combine(
                projectDirectory,
                "Output",
                method.Name);

            return new PathInfo(
                directory: directory,
                typeName: string.Empty,
                methodName: "Out");
        });
        VerifySourceGenerators.Initialize();
    }
}
