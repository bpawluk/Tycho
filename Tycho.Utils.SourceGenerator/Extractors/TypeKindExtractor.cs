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
                TypeKind.Class => Models.System.TypeKind.Class,
                TypeKind.Struct => Models.System.TypeKind.Struct,
                TypeKind.Interface => Models.System.TypeKind.Interface,
                TypeKind.Enum => Models.System.TypeKind.Enum,
                _ => Models.System.TypeKind.Other,
            };
        }
    }
}
