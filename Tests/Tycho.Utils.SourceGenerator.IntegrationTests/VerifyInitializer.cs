using System.Runtime.CompilerServices;

namespace Tycho.Utils.SourceGenerator.IntegrationTests;

internal static class VerifyInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        DerivePathInfo((sourceFile, projectDirectory, type, method) =>
        {
            var directory = Path.Combine(
                projectDirectory,
                "Output",
                method.Name);

            return new PathInfo(
                directory: directory,
                typeName: type.Name,
                methodName: method.Name);
        });
        VerifySourceGenerators.Initialize();
    }
}
