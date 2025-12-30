using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using Tycho.Utils.SourceGenerator.Model;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator
{
    /// <summary>
    /// Augments classes marked with <c>AppDefinitionAttribute</c>
    /// </summary>
    [Generator]
    public class AppDefinitionSourceGenerator : IIncrementalGenerator
    {
        /// <inheritdoc/>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: "Tycho.Apps.AppDefinitionAttribute",
                predicate: GeneratorPredicate,
                transform: BuildGeneratorModel
            );
            context.RegisterSourceOutput(pipeline, GenerateSources);
        }

        private static bool GeneratorPredicate(SyntaxNode _, CancellationToken __)
        {
            return true;
        }

        private static TychoDefinitionModel BuildGeneratorModel(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            var appClass = context.TargetSymbol;
            var appNamespace = appClass
                .ContainingNamespace
                .ToDisplayString(SymbolDisplayFormat
                    .FullyQualifiedFormat
                    .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));
            return new TychoDefinitionModel(appNamespace, appClass.Name);
        }

        private static void GenerateSources(SourceProductionContext context, TychoDefinitionModel model)
        {
            GenerateSource(context, model, "Templates/EventDispatcher.sbncs", $"{model.SourceClassName}EventDispatcher.g.cs");
            GenerateSource(context, model, "Templates/AppDefinition.sbncs", $"{model.SourceClassName}.setup.g.cs");
        }

        private static void GenerateSource(
            SourceProductionContext context, 
            TychoDefinitionModel model, 
            string templatePath, 
            string targetFileName)
        {
            var template = Template.Parse(EmbeddedResource.GetContent(templatePath), templatePath);
            var output = template.Render(model);
            var sourceText = SourceText.From(output, Encoding.UTF8);
            context.AddSource(targetFileName, sourceText);
        }
    }
}
