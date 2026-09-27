using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.TemplateModels;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.TemplateModels;

public sealed class TemplateModelBaseTests
{
    private static readonly string[] s_expected = ["where T : class"];

    [Fact]
    public void UseConstraintClauses_IgnoresNullAndEmptyConstraints()
    {
        var probe = new TemplateModelProbe();
        var constrained = new TypeParameterModel(
            "T",
            ModelHelpers.Items(TypeParameterConstraintModel.ReferenceType));
        var unconstrained = new TypeParameterModel("U", ImmutableEquatableArray<TypeParameterConstraintModel>.Empty);

        Assert.Empty(probe.ConstraintClauses(null!));
        Assert.Equal(s_expected, probe.ConstraintClauses([constrained, unconstrained]));
    }

    [Fact]
    public void UseContainingTypes_MapsDeclarationAndConstraintClauses()
    {
        var probe = new TemplateModelProbe();
        TypeDefinitionModel outer = ModelHelpers.TypeDefinition(
            "Outer",
            typeParameters: ModelHelpers.Items(new TypeParameterModel(
                "T",
                ModelHelpers.Items(TypeParameterConstraintModel.ReferenceType))));

        ContainingTypeTM result = Assert.Single(probe.ContainingTypes(ModelHelpers.Items(outer)));

        Assert.Equal("class Outer<T>", result.Declaration);
        Assert.Equal(s_expected, result.Constraints);
    }

    private sealed class TemplateModelProbe : TemplateModelBase
    {
        public IEnumerable<string> ConstraintClauses(IEnumerable<TypeParameterModel> parameters) =>
            UseConstraintClauses(parameters);

        public ContainingTypeTM[] ContainingTypes(ImmutableEquatableArray<TypeDefinitionModel> types) =>
            UseContainingTypes(types);
    }
}
