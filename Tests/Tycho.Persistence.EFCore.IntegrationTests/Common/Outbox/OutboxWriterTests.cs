using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.TestUtils.Events;
using Tycho.Persistence.EFCore.TestUtils.Other;
using Tycho.Structure;

namespace Tycho.Persistence.EFCore.IntegrationTests.Common.Outbox;

public abstract class OutboxWriterTests : IntegrationTestsBase
{
    private readonly TestDbContext _dbContext;
    private readonly Mock<IEventSerializer> _eventSerializer;
    private readonly OutboxActivity _outboxActivity;
    private readonly OutboxWriter _sut;
    private readonly PersistenceOwner _persistenceOwner;
    private int _notificationCount;

    protected OutboxWriterTests(IntegrationTestsDatabaseBase database) : base(database)
    {
        _outboxActivity = new OutboxActivity();
        _outboxActivity.NewEntriesAdded += (_, _) => _notificationCount++;
        DbContextOptions<TestDbContext> options = Database.CreateOptionsBuilder<TestDbContext>()
            .Options;
        _dbContext = new TestDbContext(options);

        _eventSerializer = new Mock<IEventSerializer>();
        _eventSerializer.Setup(serializer => serializer.Serialize(It.IsAny<RoutedEvent>()))
            .Returns((RoutedEvent routedEvent) => new SerializedRoutedEvent(
                routedEvent.Id,
                routedEvent.PublishId,
                routedEvent.EventId,
                routedEvent.HandlerId,
                routedEvent.DestinationId,
                "{}"));

        using Internals internals = PersistenceTestInternals.Create(typeof(PersistenceOwner));
        _persistenceOwner = new PersistenceOwner(internals);
        _sut = new OutboxWriter(_eventSerializer.Object, _outboxActivity, _dbContext, _persistenceOwner);
    }

    [Fact]
    public async Task Write_WithNullCollectionAndCancelledToken_ThrowsArgumentNullException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);

        // Act
        Task Act() => _sut.Write(null!, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>("routedEvents", Act);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Write_WhenSaveFails_DetachesOnlyNewEntriesAndPropagatesFailure(bool cancelSave)
    {
        // Arrange
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        Exception saveFailure = cancelSave
            ? new OperationCanceledException(cancellation.Token)
            : new InvalidOperationException("Save failed.");

        var interceptor = new Mock<SaveChangesInterceptor> { CallBase = true };
        interceptor
            .Setup(value => value.SavingChangesAsync(It.IsAny<DbContextEventData>(), It.IsAny<InterceptionResult<int>>(), cancellation.Token))
            .Returns(() =>
            {
                if (cancelSave)
                {
                    cancellation.Cancel();
                }

                return ValueTask.FromException<InterceptionResult<int>>(saveFailure);
            });

        await using var dbContext = new TestDbContext(Database.CreateOptionsBuilder<TestDbContext>().AddInterceptors(interceptor.Object).Options);
        var existingEntry = new OutboxEntry { OwnerId = _persistenceOwner.Identifier, EntryId = Guid.NewGuid() };
        dbContext.Set<OutboxEntry>().Add(existingEntry);
        var sut = new OutboxWriter(_eventSerializer.Object, _outboxActivity, dbContext, _persistenceOwner);
        RoutedEvent[] routedEvents = [CreateRoutedEvent(), CreateRoutedEvent()];

        // Act
        Exception? actual = await Record.ExceptionAsync(() => sut.Write(routedEvents, cancellation.Token));

        // Assert
        Assert.Same(saveFailure, actual);
        EntityEntry<OutboxEntry> remainingEntry = Assert.Single(dbContext.ChangeTracker.Entries<OutboxEntry>());
        Assert.Same(existingEntry, remainingEntry.Entity);
        Assert.Equal(EntityState.Added, remainingEntry.State);
        Assert.Equal(0, _notificationCount);
    }

    [Fact(Timeout = 60000)]
    public async Task Write_WithoutTransaction_SavesEntriesAndNotifies()
    {
        // Arrange
        RoutedEvent[] events =
        [
            CreateRoutedEvent("module:first"),
            CreateRoutedEvent("module:second"),
            CreateRoutedEvent("module:third")
        ];

        // Act
        await _sut.Write(events, TestContext.Current.CancellationToken);

        // Assert
        _eventSerializer.Verify(serializer => serializer.Serialize(It.IsAny<RoutedEvent>()), Times.Exactly(events.Length));
        Assert.Equal(1, _notificationCount);
        Assert.Equal(events.Length, await CountPersistedEntries());

        List<OutboxEntry> entries = await _dbContext
            .Set<OutboxEntry>()
            .AsNoTracking()
            .ToListAsync(TestContext.Current.CancellationToken);

        foreach (RoutedEvent routedEvent in events)
        {
            OutboxEntry entry = Assert.Single(entries, entry => entry.EntryId == routedEvent.Id);
            Assert.Equal(_persistenceOwner.Identifier, entry.OwnerId);
            Assert.Equal(routedEvent.Id, entry.EntryId);
            Assert.Equal(routedEvent.PublishId, entry.PublishId);
            Assert.Equal(routedEvent.EventId.Value, entry.Event);
            Assert.Equal(routedEvent.HandlerId.Value, entry.Handler);
            Assert.Equal(routedEvent.DestinationId.Value, entry.Destination);
            Assert.Equal("{}", entry.Payload);
            Assert.Equal(EntryState.New, entry.State);
        }
    }

    [Fact(Timeout = 60000)]
    public async Task Write_WhenSerializationCancels_ThrowsWithoutStagingEntriesInActiveTransaction()
    {
        // Arrange
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        RoutedEvent routed = CreateRoutedEvent();
        _eventSerializer.Setup(serializer => serializer.Serialize(routed))
            .Returns(() =>
            {
                cancellation.Cancel();
                return new SerializedRoutedEvent(routed.Id, routed.PublishId, routed.EventId, routed.HandlerId, routed.DestinationId, "{}");
            });
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);

        // Act
        Task Act() => _sut.Write([routed], cancellation.Token);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(Act);
        Assert.Empty(_dbContext.ChangeTracker.Entries<OutboxEntry>());
        Assert.Equal(0, _notificationCount);
    }

    [Fact(Timeout = 60000)]
    public async Task Write_WithEmptyBatch_DoesNotStageEntries()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        // Act
        await _sut.Write([], cancellationToken);

        // Assert
        Assert.Empty(_dbContext.ChangeTracker.Entries<OutboxEntry>());
    }

    [Fact(Timeout = 60000)]
    public async Task Write_WithinExplicitTransactionWithAsyncCommit_DefersSavingAndNotifiesOnceAfterCommit()
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
        Assert.Equal(0, await CountPersistedEntries());
        Assert.Equal(0, _notificationCount);

        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        Assert.Equal(0, _notificationCount);

        await transaction.CommitAsync(TestContext.Current.CancellationToken);

        Assert.Equal(1, _notificationCount);
        Assert.Equal(2, await CountPersistedEntries());
    }

    [Fact(Timeout = 60000)]
    public async Task Write_WithinExplicitTransactionWithSyncCommit_DefersSavingAndNotifiesOnceAfterCommit()
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

        transaction.Commit();

        Assert.Equal(1, _notificationCount);
        Assert.Equal(2, await CountPersistedEntries());
    }

    [Fact(Timeout = 60000)]
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

    [Fact(Timeout = 60000)]
    public async Task Write_WithinTychoTransaction_NotifiesAfterCommit()
    {
        // Arrange
        EFCore.Transactions.Transaction transaction = new(_dbContext);

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

    [Fact(Timeout = 60000)]
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

    [Fact(Timeout = 60000)]
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
                    routedEvent.DestinationId,
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

    private static RoutedEvent<TestEvent> CreateRoutedEvent(string destination = "test-endpoint") => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        EventIdentity.Create<TestEvent>(),
        EventHandlerIdentity.Parse($"handler-{Guid.NewGuid():N}"),
        InstanceIdentity.Parse(destination),
        new TestEvent());

    public override async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);
}
