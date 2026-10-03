using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Moq;
using Tycho.Hosting.Files;

namespace Tycho.UnitTests.Hosting.Files;

public sealed class NonDisposingFileProviderTests
{
    [Fact]
    public void Constructor_WhenProviderIsNull_Throws()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
            () => new NonDisposingFileProvider(null!));

        Assert.Equal("fileProvider", exception.ParamName);
    }

    [Fact]
    public void FileOperations_DelegateToWrappedProvider()
    {
        // Arrange
        var provider = new Mock<IFileProvider>();
        IFileInfo file = new Mock<IFileInfo>().Object;
        IDirectoryContents directory = new Mock<IDirectoryContents>().Object;
        IChangeToken changeToken = new Mock<IChangeToken>().Object;
        provider.Setup(value => value.GetFileInfo("file.txt")).Returns(file);
        provider.Setup(value => value.GetDirectoryContents("folder")).Returns(directory);
        provider.Setup(value => value.Watch("*.txt")).Returns(changeToken);
        var sut = new NonDisposingFileProvider(provider.Object);

        // Act and Assert
        Assert.Same(file, sut.GetFileInfo("file.txt"));
        Assert.Same(directory, sut.GetDirectoryContents("folder"));
        Assert.Same(changeToken, sut.Watch("*.txt"));
        provider.Verify(value => value.GetFileInfo("file.txt"), Times.Once);
        provider.Verify(value => value.GetDirectoryContents("folder"), Times.Once);
        provider.Verify(value => value.Watch("*.txt"), Times.Once);
    }
}
