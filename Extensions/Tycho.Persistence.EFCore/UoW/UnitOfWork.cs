using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Events;
using Tycho.Events.Publishing;

namespace Tycho.Persistence.EFCore.UoW;

internal class UnitOfWork(IUncommittedEventPublisher publisher, TychoDbContext dbContext) : IUnitOfWork
{
    private readonly IUncommittedEventPublisher _publisher = publisher;
    private readonly TychoDbContext _dbContext = dbContext;

    public DbSet<TEntity> Set<TEntity>() where TEntity : class
    {
        return _dbContext.Set<TEntity>();
    }

    public Task Publish<TEvent>(TEvent eventData, CancellationToken cancellationToken)
        where TEvent : class, IEvent
    {
        return _publisher.PublishWithoutCommitting(eventData, cancellationToken);
    }

    public Task SaveChanges(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}