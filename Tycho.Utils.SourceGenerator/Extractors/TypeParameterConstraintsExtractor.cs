using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class TypeParameterConstraintsExtractor
    {
        public static ImmutableEquatableArray<TypeParameterConstraintModel> Extract(ITypeParameterSymbol typeParameterSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var constraints = new List<TypeParameterConstraintModel>();

            if (typeParameterSymbol.HasUnmanagedTypeConstraint)
            {
                constraints.Add(TypeParameterConstraintModel.Unmanaged);
            }

            if (typeParameterSymbol.HasValueTypeConstraint)
            {
                constraints.Add(TypeParameterConstraintModel.ValueType);
            }

            if (typeParameterSymbol.HasReferenceTypeConstraint)
            {
                TypeParameterConstraintModel referenceTypeConstraint = typeParameterSymbol.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated
                    ? TypeParameterConstraintModel.NullableReferenceType
                    : TypeParameterConstraintModel.ReferenceType;
                constraints.Add(referenceTypeConstraint);
            }

            if (typeParameterSymbol.HasNotNullConstraint)
            {
                constraints.Add(TypeParameterConstraintModel.NotNull);
            }

            if (typeParameterSymbol.HasConstructorConstraint)
            {
                constraints.Add(TypeParameterConstraintModel.Constructor);
            }

            if (typeParameterSymbol.AllowsRefLikeType)
            {
                constraints.Add(TypeParameterConstraintModel.AllowsRefStruct);
            }

            foreach (ITypeSymbol constraintType in typeParameterSymbol.ConstraintTypes)
            {
                constraints.Add(TypeParameterConstraintModel.TypeConstraint(
                    TypeModelExtractor.Extract(constraintType, context)));
            }

            return constraints.ToImmutableEquatableArray();
        }
    }
}
