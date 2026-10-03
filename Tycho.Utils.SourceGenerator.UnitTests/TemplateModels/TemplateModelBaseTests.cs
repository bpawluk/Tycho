using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.TemplateModels;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.TemplateModels;

public sealed class TemplateModelBaseTests
{
    [Fact]
    public void UseConstraintClauses_ReturnsEmptyForNullParameters()
    {
        // Arrange
        var probe = new TemplateModelProbe();

        // Act
        string[] result = [.. probe.ConstraintClauses(null!)];

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void UseConstraintClauses_SkipsUnconstrainedParameters()
    {
        // Arrange
        var probe = new TemplateModelProbe();
        var constrainedParameter = new TypeParameterModel(
            "T",
            ModelHelpers.Items(TypeParameterConstraintModel.ReferenceType));
        var unconstrainedParameter = new TypeParameterModel(
            "U", ImmutableEquatableArray<TypeParameterConstraintModel>.Empty);

        // Act
        string[] result = [.. probe.ConstraintClauses([constrainedParameter, unconstrainedParameter])];

        // Assert
        string item = Assert.Single(result);
        Assert.Equal("where T : class", item);
    }

    [Fact]
    public void UseContainingTypes_MapsDeclarationAndConstraintClauses()
    {
        // Arrange
        var probe = new TemplateModelProbe();
        TypeDefinitionModel outer = ModelHelpers.TypeDefinition(
            "Outer",
            typeParameters: ModelHelpers.Items(new TypeParameterModel(
                "T",
                ModelHelpers.Items(TypeParameterConstraintModel.ReferenceType))));

        // Act
        ContainingTypeTM[] result = probe.ContainingTypes(ModelHelpers.Items(outer));

        // Assert
        ContainingTypeTM item = Assert.Single(result);
        Assert.Equal("class Outer<T>", item.Declaration);

        string constraints = Assert.Single(result[0].Constraints);
        Assert.Equal("where T : class", constraints);
    }

    private sealed class TemplateModelProbe : TemplateModelBase
    {
        public IEnumerable<string> ConstraintClauses(IEnumerable<TypeParameterModel> parameters) =>
            UseConstraintClauses(parameters);

        public ContainingTypeTM[] ContainingTypes(ImmutableEquatableArray<TypeDefinitionModel> types) =>
            UseContainingTypes(types);
    }
}
