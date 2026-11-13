using Microsoft.EntityFrameworkCore;
using Moq;
using Tycho.Persistence.EFCore.Transactions;

namespace Tycho.Persistence.EFCore.UnitTests.Transactions;

public sealed class TransactionTests
{
    [Fact]
    public async Task ExecuteAsync_WithNullOperation_ThrowsBeforeAccessingDependencies()
    {
        // Arrange
        var context = new Mock<TychoDbContext>(MockBehavior.Strict, new DbContextOptionsBuilder().Options);
        var sut = new Transaction(context.Object);

        // Act
        Task Act() => sut.ExecuteAsync(null!, TestContext.Current.CancellationToken);

        // Assert
        ArgumentNullException error = await Assert.ThrowsAsync<ArgumentNullException>(Act);
        Assert.Equal("operation", error.ParamName);
        context.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsyncForResult_WithNullOperation_ThrowsBeforeAccessingDependencies()
    {
        // Arrange
        var context = new Mock<TychoDbContext>(MockBehavior.Strict, new DbContextOptionsBuilder().Options);
        var sut = new Transaction(context.Object);

        // Act
        Task Act() => sut.ExecuteAsync<int>(null!, TestContext.Current.CancellationToken);

        // Assert
        ArgumentNullException error = await Assert.ThrowsAsync<ArgumentNullException>(Act);
        Assert.Equal("operation", error.ParamName);
        context.VerifyNoOtherCalls();
    }
}
