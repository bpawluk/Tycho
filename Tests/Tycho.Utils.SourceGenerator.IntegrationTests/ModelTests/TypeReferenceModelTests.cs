using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.ModelTests;

public class TypeReferenceModelTests
{
    [Fact]
    public void FullReferenceNameQualifiesContainingTypesAndGenericArguments()
    {
        var payload = new TypeReferenceModel("Other.Contracts", "Payload");
        var containingType = new TypeReferenceModel(
            "Example.Contracts",
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            "Container",
            new ImmutableEquatableArray<TypeArgumentModel>(
            [new TypeArgumentModel("TPayload", payload)]));
        var subject = new TypeReferenceModel(
            "Example.Contracts",
            new ImmutableEquatableArray<TypeReferenceModel>([containingType]),
            "Request",
            new ImmutableEquatableArray<TypeArgumentModel>(
            [new TypeArgumentModel("TResult", new TypeReferenceModel(string.Empty, "GlobalResult"))]));

        Assert.Equal(
            "global::Example.Contracts.Container<global::Other.Contracts.Payload>.Request<global::GlobalResult>",
            subject.FullReferenceName);
    }

    [Fact]
    public void FullReferenceNameDoesNotQualifyTypeParameters()
    {
        TypeReferenceModel subject = TypeReferenceModel.TypeParameter("Example.Contracts", "T");

        Assert.Equal("T", subject.FullReferenceName);
    }

    [Fact]
    public void TypeConstraintsUseFullyQualifiedReferenceNames()
    {
        var constraintType = new TypeReferenceModel(
            "Example.Contracts",
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            "IConstraint",
            new ImmutableEquatableArray<TypeArgumentModel>(
            [new TypeArgumentModel("T", TypeReferenceModel.TypeParameter("Test", "T"))]));
        var subject = new TypeParameterModel(
            "T",
            new ImmutableEquatableArray<TypeParameterConstraintModel>(
            [TypeParameterConstraintModel.TypeConstraint(constraintType)]));

        Assert.Equal("where T : global::Example.Contracts.IConstraint<T>", subject.ConstraintsClause);
    }
}
