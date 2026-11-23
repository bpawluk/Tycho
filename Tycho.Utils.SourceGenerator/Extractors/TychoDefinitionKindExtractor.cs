using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Extensions;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class TychoDefinitionKindExtractor
    {
        public static TychoDefinitionKind Extract(ITypeSymbol typeSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            INamedTypeSymbol tychoAppSymbol = context.Compilation.GetTypeByMetadataName(TychoAppReference.FullName);
            if (tychoAppSymbol != null && typeSymbol.InheritsFrom(tychoAppSymbol))
            {
                return TychoDefinitionKind.App;
            }

            INamedTypeSymbol tychoModuleSymbol = context.Compilation.GetTypeByMetadataName(TychoModuleReference.FullName);
            if (tychoModuleSymbol != null && typeSymbol.InheritsFrom(tychoModuleSymbol))
            {
                return TychoDefinitionKind.Module;
            }

            return TychoDefinitionKind.Unknown;
        }
    }
}
