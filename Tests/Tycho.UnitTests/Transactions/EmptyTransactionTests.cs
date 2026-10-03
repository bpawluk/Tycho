using Tycho.Transactions;

namespace Tycho.UnitTests.Transactions;

public sealed class EmptyTransactionTests
{
    [Fact]
    public void ExecuteAsync_WithoutProvider_ThrowsBeforeInvokingOperation()
    {
        // Arrange
        var sut = new EmptyTransaction();
        bool operationInvoked = false;

        // Act
        void Act() => sut.ExecuteAsync(_ =>
        {
            operationInvoked = true;
            return Task.CompletedTask;
        }, TestContext.Current.CancellationToken);

        // Assert
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(Act);
        Assert.Equal("No transaction provider is configured.", exception.Message);
        Assert.False(operationInvoked);
    }
}
