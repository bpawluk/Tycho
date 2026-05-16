using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Tycho.Events.Model;
using Tycho.Persistence.EFCore.Common;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UnitTests.Outbox;

public class OutboxConsumerTests
{
    private readonly Mock<ITransaction> _transactionMock;
    private readonly Mock<TychoDbContext> _dbContextMock;
    private readonly OutboxConsumerSettings _settings;

    private readonly Mock<DbSet<OutboxEntry>> _dbSetMock;

    private readonly OutboxConsumer _sut;

    public OutboxConsumerTests()
    {
        _transactionMock = new Mock<ITransaction>();

        _settings = new OutboxConsumerSettings
        {
            MaxDeliveryCount = 3,
            DeliveryExpiration = TimeSpan.FromMinutes(5)
        };

        _dbSetMock = new Mock<DbSet<OutboxEntry>>();

        _dbContextMock = new Mock<TychoDbContext>();
        _dbContextMock.Setup(db => db.Set<OutboxEntry>())
                      .Returns(_dbSetMock.Object);
        _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(0);

        _sut = new OutboxConsumer(_transactionMock.Object, _dbContextMock.Object, _settings);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Read_WithNewEntries_ReadsTheEntries(bool isTransactionInProgress)
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        _transactionMock.Setup(t => t.IsInProgress)
                        .Returns(isTransactionInProgress);

        List<OutboxEntry> entries =
        [
            CreateEntry(Guid.NewGuid(), EntryState.New, 0, DateTime.UtcNow - TimeSpan.FromMinutes(3)),
            CreateEntry(Guid.NewGuid(), EntryState.New, 0, DateTime.UtcNow - TimeSpan.FromMinutes(2)),
            CreateEntry(Guid.NewGuid(), EntryState.New, 0, DateTime.UtcNow - TimeSpan.FromMinutes(1)),
        ];

        _dbContextMock.Setup(db => db.Set<OutboxEntry>())
                      .ReturnsDbSet(entries);

        // Act
        IReadOnlyCollection<SerializedRoutedEvent> result = await _sut.Read(entries.Count, cancellationToken);

        // Assert
        Assert.Equal(entries.Count, result.Count);
        Assert.All(entries, e => Assert.Contains(result, r => r.Id == e.Id));
        Assert.All(entries, e => Assert.Equal(EntryState.InProcessing, e.State));
        Assert.All(entries, e => Assert.Equal(1u, e.DeliveryAttempts));
        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), isTransactionInProgress ? Times.Never() : Times.Once());
    }

    [Fact]
    public async Task Read_WithInProcessingEntriesAfterExpirationBelowMaxDeliveryCount_ReadsTheEntries()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        List<OutboxEntry> entries =
        [
            CreateEntry(Guid.NewGuid(), EntryState.InProcessing, 0, DateTime.UtcNow - _settings.DeliveryExpiration * 3),
            CreateEntry(Guid.NewGuid(), EntryState.InProcessing, 1, DateTime.UtcNow - _settings.DeliveryExpiration * 2),
            CreateEntry(Guid.NewGuid(), EntryState.InProcessing, 2, DateTime.UtcNow - _settings.DeliveryExpiration * 1.1),
        ];

        _dbContextMock.Setup(db => db.Set<OutboxEntry>())
                      .ReturnsDbSet(entries);

        // Act
        IReadOnlyCollection<SerializedRoutedEvent> result = await _sut.Read(entries.Count, cancellationToken);

        // Assert
        Assert.Equal(entries.Count, result.Count);
        Assert.All(entries, e => Assert.Contains(result, r => r.Id == e.Id));
        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Read_WithInProcessingEntriesBeforeExpiration_IgnoresTheEntries()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        List<OutboxEntry> entries =
        [
            CreateEntry(Guid.NewGuid(), EntryState.InProcessing, 0, DateTime.UtcNow - _settings.DeliveryExpiration * 0.3),
            CreateEntry(Guid.NewGuid(), EntryState.InProcessing, 1, DateTime.UtcNow - _settings.DeliveryExpiration * 0.2),
            CreateEntry(Guid.NewGuid(), EntryState.InProcessing, 2, DateTime.UtcNow - _settings.DeliveryExpiration * 0.1),
        ];

        _dbContextMock.Setup(db => db.Set<OutboxEntry>())
                      .ReturnsDbSet(entries);

        // Act
        IReadOnlyCollection<SerializedRoutedEvent> result = await _sut.Read(entries.Count, cancellationToken);

        // Assert
        Assert.Empty(result);
        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Read_WithInProcessingEntriesAboveMaxDeliveryCount_IgnoresTheEntries()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        List<OutboxEntry> entries =
        [
            CreateEntry(Guid.NewGuid(), EntryState.InProcessing, _settings.MaxDeliveryCount, DateTime.UtcNow - _settings.DeliveryExpiration * 3),
            CreateEntry(Guid.NewGuid(), EntryState.InProcessing, _settings.MaxDeliveryCount, DateTime.UtcNow - _settings.DeliveryExpiration * 2),
            CreateEntry(Guid.NewGuid(), EntryState.InProcessing, _settings.MaxDeliveryCount, DateTime.UtcNow - _settings.DeliveryExpiration * 1.1),
        ];

        _dbContextMock.Setup(db => db.Set<OutboxEntry>())
                      .ReturnsDbSet(entries);

        // Act
        IReadOnlyCollection<SerializedRoutedEvent> result = await _sut.Read(entries.Count, cancellationToken);

        // Assert
        Assert.Empty(result);
        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Read_WithFailedEntriesBelowMaxDeliveryCount_ReadsTheEntries()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        List<OutboxEntry> entries =
        [
            CreateEntry(Guid.NewGuid(), EntryState.Failed, 0, DateTime.UtcNow - TimeSpan.FromMinutes(3)),
            CreateEntry(Guid.NewGuid(), EntryState.Failed, 1, DateTime.UtcNow - TimeSpan.FromMinutes(2)),
            CreateEntry(Guid.NewGuid(), EntryState.Failed, 2, DateTime.UtcNow - TimeSpan.FromMinutes(1)),
        ];

        _dbContextMock.Setup(db => db.Set<OutboxEntry>())
                      .ReturnsDbSet(entries);

        // Act
        IReadOnlyCollection<SerializedRoutedEvent> result = await _sut.Read(entries.Count, cancellationToken);

        // Assert
        Assert.Equal(entries.Count, result.Count);
        Assert.All(entries, e => Assert.Contains(result, r => r.Id == e.Id));
        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Read_WithFailedEntriesAboveMaxDeliveryCount_IgnoresTheEntries()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        List<OutboxEntry> entries =
        [
            CreateEntry(Guid.NewGuid(), EntryState.Failed, _settings.MaxDeliveryCount, DateTime.UtcNow - TimeSpan.FromMinutes(3)),
            CreateEntry(Guid.NewGuid(), EntryState.Failed, _settings.MaxDeliveryCount, DateTime.UtcNow - TimeSpan.FromMinutes(2)),
            CreateEntry(Guid.NewGuid(), EntryState.Failed, _settings.MaxDeliveryCount, DateTime.UtcNow - TimeSpan.FromMinutes(1)),
        ];

        _dbContextMock.Setup(db => db.Set<OutboxEntry>())
                      .ReturnsDbSet(entries);

        // Act
        IReadOnlyCollection<SerializedRoutedEvent> result = await _sut.Read(entries.Count, cancellationToken);

        // Assert
        Assert.Empty(result);
        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task MarkAsDelivered_WithExistingEntry_UpdatesItsState(bool isTransactionInProgress)
    {
        // Arrange
        var entryId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(entryId, EntryState.InProcessing, 1, DateTime.UtcNow);
        var cancellationToken = new CancellationToken();

        _transactionMock.Setup(t => t.IsInProgress)
                        .Returns(isTransactionInProgress);

        _dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                  .Returns(ValueTask.FromResult<OutboxEntry?>(entry));

        // Act
        await _sut.MarkAsDelivered(entryId, cancellationToken);

        // Assert
        Assert.Equal(EntryState.Processed, entry.State);
        Assert.Equal(DateTime.UtcNow, entry.Updated, TimeSpan.FromSeconds(1));
        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), isTransactionInProgress ? Times.Never() : Times.Once());
    }

    [Fact]
    public async Task MarkAsDelivered_WithMissingEntry_IgnoresTheRequest()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                  .Returns(ValueTask.FromResult<OutboxEntry?>(null));

        // Act
        await _sut.MarkAsDelivered(entryId, cancellationToken);

        // Assert
        _dbSetMock.Verify(m => m.Remove(It.IsAny<OutboxEntry>()), Times.Never);
        _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task MarkAsFailed_WithExistingEntry_UpdatesItsState(bool isTransactionInProgress)
    {
        // Arrange
        var entryId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(entryId, EntryState.InProcessing, 1, DateTime.UtcNow - TimeSpan.FromMinutes(5));
        var cancellationToken = new CancellationToken();

        _transactionMock.Setup(t => t.IsInProgress)
                        .Returns(isTransactionInProgress);

        _dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                  .Returns(ValueTask.FromResult<OutboxEntry?>(entry));

        // Act
        await _sut.MarkAsFailed(entryId, cancellationToken);

        // Assert
        Assert.Equal(EntryState.Failed, entry.State);
        Assert.Equal(DateTime.UtcNow, entry.Updated, TimeSpan.FromSeconds(1));
        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), isTransactionInProgress ? Times.Never() : Times.Once());
    }

    [Fact]
    public async Task MarkAsFailed_WithMissingEntry_IgnoresTheRequest()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                  .Returns(ValueTask.FromResult<OutboxEntry?>(null));

        // Act
        await _sut.MarkAsFailed(entryId, cancellationToken);

        // Assert
        _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static OutboxEntry CreateEntry(Guid id, EntryState state, uint deliveryAttempts, DateTime updated) => new()
    {
        Id = id,
        State = state,
        DeliveryAttempts = deliveryAttempts,
        Updated = updated,
        Created = updated - TimeSpan.FromMinutes(1),
        Event = "test-event",
        Handler = "test-handler",
        Route = "END",
        Payload = "{}"
    };
}
