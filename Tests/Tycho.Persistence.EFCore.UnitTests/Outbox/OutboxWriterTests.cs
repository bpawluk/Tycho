using System.Transactions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.UnitTests._Data.Events;

namespace Tycho.Persistence.EFCore.UnitTests.Outbox;

public sealed class OutboxWriterTests : IAsyncLifetime
{
    private SqliteConnection _connection = default!;
    private TestDbContext _dbContext = default!;
    private Mock<IEventSerializer> _eventSerializer = default!;
    private OutboxActivity _outboxActivity = default!;
    private OutboxWriter _sut = default!;
    private int _notificationCount;

    public async ValueTask InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync(TestContext.Current.CancellationToken);

        _outboxActivity = new OutboxActivity();
        _outboxActivity.NewEntriesAdded += (_, _) => _notificationCount++;
        DbContextOptions<TestDbContext> options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .Options;
        _dbContext = new TestDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        _eventSerializer = new Mock<IEventSerializer>();
        _eventSerializer.Setup(serializer => serializer.Serialize(It.IsAny<RoutedEvent>()))
            .Returns((RoutedEvent routedEvent) => new SerializedRoutedEvent(
                routedEvent.Id,
                routedEvent.PublishId,
                routedEvent.EventId,
                routedEvent.HandlerId,
                routedEvent.Route,
                "{}"));

        _sut = new OutboxWriter(_eventSerializer.Object, _outboxActivity, _dbContext);
    }

    [Fact]
    public async Task Write_WithoutTransaction_SavesEntriesAndNotifies()
    {
        // Arrange
        RoutedEvent[] events = [CreateRoutedEvent(), CreateRoutedEvent(), CreateRoutedEvent()];

        // Act
        await _sut.Write(events, TestContext.Current.CancellationToken);

        // Assert
        _eventSerializer.Verify(serializer => serializer.Serialize(It.IsAny<RoutedEvent>()), Times.Exactly(events.Length));
        Assert.Equal(1, _notificationCount);
        Assert.Equal(events.Length, await CountPersistedEntries());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Write_WithinExplicitTransaction_DefersSavingAndNotifiesOnceAfterCommit(bool commitAsync)
    {
        // Arrange
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);

        // Act
        await _sut.Write([CreateRoutedEvent()], TestContext.Current.CancellationToken);
        await _sut.Write([CreateRoutedEvent()], TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, _dbContext.ChangeTracker.Entries<OutboxEntry>()
            .Count(entry => entry.State == EntityState.Added));
        Assert.Equal(0, _notificationCount);

        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        Assert.Equal(0, _notificationCount);

        if (commitAsync)
        {
            await transaction.CommitAsync(TestContext.Current.CancellationToken);
        }
        else
        {
            transaction.Commit();
        }

        Assert.Equal(1, _notificationCount);
        Assert.Equal(2, await CountPersistedEntries());
    }

    [Fact]
    public async Task Write_WithinExplicitTransaction_DoesNotNotifyOrPersistAfterRollback()
    {
        // Arrange
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);

        // Act
        await _sut.Write([CreateRoutedEvent()], TestContext.Current.CancellationToken);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        await transaction.RollbackAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, _notificationCount);
        _dbContext.ChangeTracker.Clear();
        Assert.Equal(0, await CountPersistedEntries());
    }

    [Fact]
    public async Task Write_WithinTychoTransaction_NotifiesAfterCommit()
    {
        // Arrange
        var transaction = new Tycho.Persistence.EFCore.Transactions.Transaction(_dbContext);

        // Act
        await transaction.ExecuteAsync(async cancellationToken =>
        {
            await _sut.Write([CreateRoutedEvent()], cancellationToken);
            Assert.Equal(0, _notificationCount);
        }, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, _notificationCount);
        Assert.Equal(1, await CountPersistedEntries());
    }

    [Fact]
    public async Task Write_WithinAmbientTransaction_DefersSavingAndNotification()
    {
        // Act
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await _sut.Write([CreateRoutedEvent()], TestContext.Current.CancellationToken);
            EntityEntry<OutboxEntry> item = Assert.Single(_dbContext.ChangeTracker.Entries<OutboxEntry>());
            Assert.Equal(EntityState.Added, item.State);
            Assert.Equal(0, _notificationCount);
        }

        // Assert
        _dbContext.ChangeTracker.Clear();
        Assert.Equal(0, await CountPersistedEntries());
        Assert.Equal(0, _notificationCount);
    }

    [Fact]
    public async Task Write_WhenSerializationFails_DoesNotTrackPartialEntriesOrNotify()
    {
        // Arrange
        int serializationCount = 0;
        _eventSerializer.Setup(serializer => serializer.Serialize(It.IsAny<RoutedEvent>()))
            .Returns((RoutedEvent routedEvent) =>
            {
                if (++serializationCount == 2)
                {
                    throw new InvalidOperationException("serialization failure");
                }

                return new SerializedRoutedEvent(
                    routedEvent.Id,
                    routedEvent.PublishId,
                    routedEvent.EventId,
                    routedEvent.HandlerId,
                    routedEvent.Route,
                    "{}");
            });

        // Act
        Task Act() => _sut.Write(
            [CreateRoutedEvent(), CreateRoutedEvent()],
            TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Empty(_dbContext.ChangeTracker.Entries<OutboxEntry>());
        Assert.Equal(0, _notificationCount);
    }

    private async Task<int> CountPersistedEntries() =>
        await _dbContext.Set<OutboxEntry>().AsNoTracking().CountAsync(TestContext.Current.CancellationToken);

    private static RoutedEvent CreateRoutedEvent() => new RoutedEvent<TestEvent>(
        Guid.NewGuid(),
        Guid.NewGuid(),
        EventIdentity.Create<TestEvent>(),
        EventHandlerIdentity.Parse($"handler-{Guid.NewGuid():N}"),
        Route.Create(),
        new TestEvent());

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);
}
