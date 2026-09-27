using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests._Utils;

internal sealed class RoslynTestCompilation
{
    private RoslynTestCompilation(CSharpCompilation compilation)
    {
        Compilation = compilation;
        Context = new ExtractorContext(
            compilation,
            new SemanticModelProvider(compilation),
            CancellationToken.None);
    }

    public CSharpCompilation Compilation { get; }

    public ExtractorContext Context { get; }

    public static RoslynTestCompilation Create(string source)
    {
        CSharpParseOptions parseOptions = new(LanguageVersion.Preview);
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions, cancellationToken: CancellationToken.None);

        string[] trustedPlatformAssemblies = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))?
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries) ?? [];

        PortableExecutableReference[] references = [.. trustedPlatformAssemblies
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(path => MetadataReference.CreateFromFile(path))];

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "SourceGeneratorUnitTestCompilation",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                allowUnsafe: true,
                nullableContextOptions: NullableContextOptions.Enable));

        return new RoslynTestCompilation(compilation);
    }

    public INamedTypeSymbol Type(string metadataName) =>
        Compilation.GetTypeByMetadataName(metadataName)
        ?? throw new InvalidOperationException($"Type '{metadataName}' was not found in the test compilation.");

    public INamedTypeSymbol DeclaredType(string name)
    {
        foreach (SyntaxTree syntaxTree in Compilation.SyntaxTrees)
        {
            SemanticModel semanticModel = Compilation.GetSemanticModel(syntaxTree);
            foreach (TypeDeclarationSyntax declaration in syntaxTree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                if (declaration.Identifier.ValueText == name && semanticModel.GetDeclaredSymbol(declaration) is INamedTypeSymbol type)
                {
                    return type;
                }
            }

            foreach (DelegateDeclarationSyntax declaration in syntaxTree.GetRoot().DescendantNodes().OfType<DelegateDeclarationSyntax>())
            {
                if (declaration.Identifier.ValueText == name && semanticModel.GetDeclaredSymbol(declaration) is INamedTypeSymbol type)
                {
                    return type;
                }
            }
        }

        throw new InvalidOperationException($"Type declaration '{name}' was not found in the test compilation.");
    }

    public IMethodSymbol Method(string containingType, string name) =>
        Type(containingType).GetMembers(name).OfType<IMethodSymbol>().Single();

    public ITypeParameterSymbol TypeParameter(string containingType, string name) =>
        Type(containingType).TypeParameters.Single(parameter => parameter.Name == name);
}
