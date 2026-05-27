using Microsoft.CodeAnalysis;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class TypeKindExtractor
    {
        public static Models.System.TypeKind Extract(ITypeSymbol typeSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (typeSymbol.IsRecord)
            {
                return typeSymbol.IsValueType
                    ? Models.System.TypeKind.RecordStruct
                    : Models.System.TypeKind.RecordClass;
            }

            return typeSymbol.TypeKind switch
            {
                TypeKind.Interface => Models.System.TypeKind.Interface,
                TypeKind.Struct => Models.System.TypeKind.Struct,
                _ => Models.System.TypeKind.Class,
            };
        }
    }
}
