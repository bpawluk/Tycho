using Microsoft.EntityFrameworkCore;
using Moq;
using Tycho.Events.Publishing;
using Tycho.Persistence.EFCore.UnitTests._Data.Entities;
using Tycho.Persistence.EFCore.UnitTests._Data.Events;
using Tycho.Persistence.EFCore.UoW;

namespace Tycho.Persistence.EFCore.UnitTests.UoW;

public class UnitOfWorkTests
{
    private readonly Mock<IUncommittedEventPublisher> _publisherMock;
    private readonly Mock<TychoDbContext> _dbContextMock;

    private readonly UnitOfWork _sut;

    public UnitOfWorkTests()
    {
        _publisherMock = new Mock<IUncommittedEventPublisher>();
        _dbContextMock = new Mock<TychoDbContext>();
        _sut = new UnitOfWork(_publisherMock.Object, _dbContextMock.Object);
    }

    [Fact]
    public void Set_ReturnsDbSetFromContext()
    {
        // Arrange
        var mockDbSet = new Mock<DbSet<TestEntity>>();
        _dbContextMock.Setup(db => db.Set<TestEntity>())
                      .Returns(mockDbSet.Object);

        // Act
        var result = _sut.Set<TestEntity>();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<DbSet<TestEntity>>(result);
    }

    [Fact]
    public async Task Publish_CallsPublisherWithoutCommitting()
    {
        // Arrange
        var eventData = new TestEvent();
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.Publish(eventData, cancellationToken);

        // Assert
        _publisherMock.Verify(p => p.PublishWithoutCommitting(eventData, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task SaveChanges_CallsContextSaveChanges()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.SaveChanges(cancellationToken);

        // Assert
        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), Times.Once);
    }
}