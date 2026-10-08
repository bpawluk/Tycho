using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models;

public sealed class TychoDefinitionModelTests
{
    [Fact]
    public void None_ReturnsInvalidDefinition()
    {
        // Act
        TychoDefinitionModel result = TychoDefinitionModel.None();

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(0, result.GetHashCode());
    }

    [Fact]
    public void Equality_NoneDefinitionsAreEqual()
    {
        // Arrange
        TychoDefinitionModel first = TychoDefinitionModel.None();
        TychoDefinitionModel equal = TychoDefinitionModel.None();
        TychoDefinitionModel valid = CreateValidDefinition();

        // Act
        bool equalWithOperator = first == equal;
        bool unequalWithOperator = first != valid;

        // Assert
        ModelHelpers.AssertValueSemantics(first, equal, valid);
        Assert.True(equalWithOperator);
        Assert.True(unequalWithOperator);
        Assert.False(first != equal);
        Assert.False(first == valid);
    }

    [Fact]
    public void Constructor_SetsValidDefinitionMembers()
    {
        // Arrange
        TypeDefinitionModel definitionType = ModelHelpers.TypeDefinition("App");
        MethodDefinitionModel contract = CreateMethod("DefineContract");
        MethodDefinitionModel events = CreateMethod("DefineEvents");
        MethodDefinitionModel modules = CreateMethod("IncludeModules");

        // Act
        var sut = new TychoDefinitionModel(TychoDefinitionKind.App, definitionType, contract, events, modules);

        // Assert
        Assert.True(sut.IsValid);
        Assert.Equal(TychoDefinitionKind.App, sut.DefinitionKind);
        Assert.Equal(definitionType, sut.DefinitionType);
        Assert.Equal(contract, sut.DefineContractMethod);
        Assert.Equal(events, sut.DefineEventsMethod);
        Assert.Equal(modules, sut.IncludeModulesMethod);
    }

    [Fact]
    public void Equality_EquivalentAndDifferentDefinitions_UsesDefinitionMembers()
    {
        // Arrange
        TychoDefinitionModel first = CreateValidDefinition();
        TychoDefinitionModel equal = CreateValidDefinition();
        TychoDefinitionModel different = new(
            TychoDefinitionKind.Module,
            ModelHelpers.TypeDefinition("App"),
            CreateMethod("DefineContract"),
            CreateMethod("DefineEvents"),
            CreateMethod("IncludeModules"));

        // Act
        bool equalWithOperator = first == equal;
        bool unequalWithOperator = first != different;

        // Assert
        ModelHelpers.AssertValueSemantics(first, equal, different);
        Assert.True(equalWithOperator);
        Assert.True(unequalWithOperator);
        Assert.False(first != equal);
        Assert.False(first == different);
    }

    private static TychoDefinitionModel CreateValidDefinition() => new(
        TychoDefinitionKind.App,
        ModelHelpers.TypeDefinition("App"),
        CreateMethod("DefineContract"),
        CreateMethod("DefineEvents"),
        CreateMethod("IncludeModules"));

    private static MethodDefinitionModel CreateMethod(string name) =>
        new(
            ModelHelpers.TypeDefinition("App"),
            ModelHelpers.MethodSignature(name),
            ModelHelpers.Items<MethodInvocationModel>());
}
