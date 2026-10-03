using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.Tycho;

public sealed class TychoRequestModelTests
{
    [Fact]
    public void Constructor_WithoutResponse_ReportsNoResponse()
    {
        // Arrange
        TypeReferenceModel request = ModelHelpers.TypeReference("Request");

        // Act
        var sut = new TychoRequestModel(request);

        // Assert
        Assert.Equal(request, sut.RequestType);
        Assert.Null(sut.ResponseType);
        Assert.False(sut.HasResponse);
    }

    [Fact]
    public void Constructor_WithResponse_ReportsResponse()
    {
        // Arrange
        TypeReferenceModel request = ModelHelpers.TypeReference("Request");
        TypeReferenceModel response = ModelHelpers.TypeReference("Response");

        // Act
        var sut = new TychoRequestModel(request, response);

        // Assert
        Assert.Equal(request, sut.RequestType);
        Assert.Equal(response, sut.ResponseType);
        Assert.True(sut.HasResponse);
    }

    [Fact]
    public void Equality_UsesRequestAndOptionalResponse()
    {
        // Arrange
        var first = new TychoRequestModel(ModelHelpers.TypeReference("Request"), ModelHelpers.TypeReference("Response"));
        var equal = new TychoRequestModel(ModelHelpers.TypeReference("Request"), ModelHelpers.TypeReference("Response"));
        var differentRequest = new TychoRequestModel(ModelHelpers.TypeReference("OtherRequest"), ModelHelpers.TypeReference("Response"));
        var differentResponse = new TychoRequestModel(ModelHelpers.TypeReference("Request"), ModelHelpers.TypeReference("OtherResponse"));
        var noResponse = new TychoRequestModel(ModelHelpers.TypeReference("Request"));

        // Act & Assert
        Assert.True(first == equal);
        Assert.False(first != equal);
        Assert.False(first == differentRequest);
        Assert.True(first != differentRequest);
        Assert.False(first.Equals(differentResponse));
        Assert.False(first.Equals(noResponse));
        Assert.Equal(first.GetHashCode(), equal.GetHashCode());
    }
}
