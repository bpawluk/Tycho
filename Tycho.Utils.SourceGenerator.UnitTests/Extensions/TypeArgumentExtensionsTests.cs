using Tycho.Utils.SourceGenerator.Extensions;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extensions;

public sealed class TypeArgumentExtensionsTests
{
    [Theory]
    [InlineData("TRequest", true, false, false, false)]
    [InlineData("TResponse", false, true, false, false)]
    [InlineData("TEvent", false, false, true, false)]
    [InlineData("TModule", false, false, false, true)]
    [InlineData("TOther", false, false, false, false)]
    public void TypeParameterNames_AreRecognizedForTheirContractKind(
        string name,
        bool isRequest,
        bool isResponse,
        bool isEvent,
        bool isModule)
    {
        var argument = new TypeArgumentModel(name, TypeReferenceModel.TypeParameter("Demo", name));

        Assert.Equal(isRequest, argument.IsRequestType());
        Assert.Equal(isResponse, argument.IsResponseType());
        Assert.Equal(isEvent, argument.IsEventType());
        Assert.Equal(isModule, argument.IsModuleType());
    }
}
