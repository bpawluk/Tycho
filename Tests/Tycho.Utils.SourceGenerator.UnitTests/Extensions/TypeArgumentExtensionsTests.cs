using Tycho.Utils.SourceGenerator.Extensions;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extensions;

public sealed class TypeArgumentExtensionsTests
{
    [Theory]
    [InlineData("TRequest", true)]
    [InlineData("TResponse", false)]
    [InlineData("TEvent", false)]
    [InlineData("TModule", false)]
    [InlineData("TOther", false)]
    public void IsRequestType_RecognizesOnlyRequestTypeParameter(string name, bool expected)
    {
        // Arrange
        var argument = new TypeArgumentModel(name, TypeReferenceModel.TypeParameter("Demo", name));

        // Act
        bool result = argument.IsRequestType();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("TRequest", false)]
    [InlineData("TResponse", true)]
    [InlineData("TEvent", false)]
    [InlineData("TModule", false)]
    [InlineData("TOther", false)]
    public void IsResponseType_RecognizesOnlyResponseTypeParameter(string name, bool expected)
    {
        // Arrange
        var argument = new TypeArgumentModel(name, TypeReferenceModel.TypeParameter("Demo", name));

        // Act
        bool result = argument.IsResponseType();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("TRequest", false)]
    [InlineData("TResponse", false)]
    [InlineData("TEvent", true)]
    [InlineData("TModule", false)]
    [InlineData("TOther", false)]
    public void IsEventType_RecognizesOnlyEventTypeParameter(string name, bool expected)
    {
        // Arrange
        var argument = new TypeArgumentModel(name, TypeReferenceModel.TypeParameter("Demo", name));

        // Act
        bool result = argument.IsEventType();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("TRequest", false)]
    [InlineData("TResponse", false)]
    [InlineData("TEvent", false)]
    [InlineData("TModule", true)]
    [InlineData("TOther", false)]
    public void IsModuleType_RecognizesOnlyModuleTypeParameter(string name, bool expected)
    {
        // Arrange
        var argument = new TypeArgumentModel(name, TypeReferenceModel.TypeParameter("Demo", name));

        // Act
        bool result = argument.IsModuleType();

        // Assert
        Assert.Equal(expected, result);
    }
}
