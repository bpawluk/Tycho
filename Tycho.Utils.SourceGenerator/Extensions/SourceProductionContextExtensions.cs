using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Tycho.Utils.SourceGenerator.Extensions
{
    internal static class SourceProductionContextExtensions
    {
        public static SourceProductionContext GenerateSourceFromTemplate(
            this SourceProductionContext context,
            object model,
            string templateContent,
            string targetFileName)
        {
            var template = Template.Parse(templateContent);
            string output = template.Render(model);
            var sourceText = SourceText.From(output, Encoding.UTF8);
            context.AddSource(targetFileName, sourceText);
            return context;
        }
    }
}
