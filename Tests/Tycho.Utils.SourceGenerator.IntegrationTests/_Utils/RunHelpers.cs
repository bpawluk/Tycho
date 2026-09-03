using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Tycho.Utils.SourceGenerator.IntegrationTests._Utils;

internal static class RunHelpers
{
    public static GeneratorDriver CreateGeneratorDriver()
    {
        var generator = new TychoSourceGenerator();
        return CSharpGeneratorDriver.Create(generator);
    }

    public static GeneratorDriver CreateGeneratorDriverWithTracking()
    {
        var generator = new TychoSourceGenerator();
        return CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            additionalTexts: null,
            parseOptions: null,
            optionsProvider: null,
            driverOptions: new GeneratorDriverOptions(
                disabledOutputs: IncrementalGeneratorOutputKind.None,
                trackIncrementalGeneratorSteps: true));
    }

    public static CSharpCompilation CreateCompilation(string sourceDir, string[] sources)
    {
        IEnumerable<SyntaxTree> syntaxTrees = sources.Select(source =>
        {
            string sourcePath = Path.Combine(AppContext.BaseDirectory, sourceDir, source);
            string sourceContent = File.ReadAllText(sourcePath);
            return CSharpSyntaxTree.ParseText(sourceContent, path: sourcePath);
        });

        string[] trustedPlatformAssemblies = ((string?)AppContext
            .GetData("TRUSTED_PLATFORM_ASSEMBLIES"))?
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries) ?? [];

        PortableExecutableReference[] references = [.. trustedPlatformAssemblies
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(path => MetadataReference.CreateFromFile(path))];

        return CSharpCompilation.Create("Compilation", syntaxTrees, references, new(OutputKind.DynamicallyLinkedLibrary));
    }

    public static GeneratorDriver RunGenerator(string sourceDir, string[] sources)
    {
        GeneratorDriver driver = CreateGeneratorDriver();
        CSharpCompilation compilation = CreateCompilation(sourceDir, sources);

        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out Compilation updatedCompilation,
            out ImmutableArray<Diagnostic> generatorDiagnostics);

        HashSet<SyntaxTree> inputSyntaxTrees = [.. compilation.SyntaxTrees];
        Diagnostic[] errors = [.. generatorDiagnostics
            .Concat(updatedCompilation.GetDiagnostics()
                .Where(diagnostic => diagnostic.Location.SourceTree is not { } sourceTree || !inputSyntaxTrees.Contains(sourceTree)))
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .DistinctBy(diagnostic => diagnostic.ToString())];

        Assert.True(
            errors.Length == 0,
            $"Generated compilation contains errors:" +
            $"{Environment.NewLine}{string.Join(Environment.NewLine, errors.Select(diagnostic => diagnostic.ToString()))}");

        return driver;
    }
}
