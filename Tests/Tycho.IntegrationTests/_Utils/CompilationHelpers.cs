using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Utils.SourceGenerator;

namespace Tycho.IntegrationTests._Utils;

internal static class CompilationHelpers
{
    public static IReadOnlyCollection<Diagnostic> CompileWithTychoGenerator(params string[] sources)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.Latest);
        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        IEnumerable<SyntaxTree> syntaxTrees = sources.Select((source, index) =>
            CSharpSyntaxTree.ParseText(source, parseOptions, path: $"source_{index}.cs"));

        CSharpCompilation compilation = CSharpCompilation.Create("Compilation", syntaxTrees, GetMetadataReferences(), options);

        var generator = new TychoSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation? outputCompilation, out ImmutableArray<Diagnostic> generatorDiagnostics);

        return [.. outputCompilation.GetDiagnostics(), .. generatorDiagnostics];
    }

    private static List<PortableExecutableReference> GetMetadataReferences()
    {
        string? trustedPlatformAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;

        var references = trustedPlatformAssemblies!
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(path => MetadataReference.CreateFromFile(path))
            .ToList();

        references.Add(MetadataReference.CreateFromFile(typeof(TychoDefinitionAttribute).GetTypeInfo().Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(TychoSourceGenerator).GetTypeInfo().Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(IServiceCollection).GetTypeInfo().Assembly.Location));

        return references;
    }
}
