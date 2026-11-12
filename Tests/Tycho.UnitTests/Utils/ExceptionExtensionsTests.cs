using Tycho.Utils;

namespace Tycho.UnitTests.Utils;

public sealed class ExceptionExtensionsTests
{
    [Fact]
    public void ThrowIfNull_WithValue_DoesNotThrow()
    {
        new object().ThrowIfNull();
    }

    [Fact]
    public void ThrowIfNull_WithNull_ReportsCallerArgumentName()
    {
        object? argument = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => argument.ThrowIfNull());

        Assert.Equal("argument", exception.ParamName);
    }

    [Fact]
    public void ThrowIfNull_WithExplicitName_ReportsThatName()
    {
        object? argument = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => argument.ThrowIfNull("customName"));

        Assert.Equal("customName", exception.ParamName);
    }

    [Fact]
    public void ThrowIfNull_WithNullName_ThrowsWithoutParameterName()
    {
        object? argument = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => argument.ThrowIfNull(null));

        Assert.Null(exception.ParamName);
    }
}
