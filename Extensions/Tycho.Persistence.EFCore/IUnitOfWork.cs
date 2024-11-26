using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Events;

namespace Tycho.Persistence.EFCore;

/// <summary>
/// Unit of work that manages database operations during a business transaction
/// </summary>
public interface IUnitOfWork : IEventPublisher
{
    /// <summary>
    /// Creates a <see cref="DbSet{TEntity}"/> that can be used to query and save instances of <typeparamref name="TEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which a set should be returned</typeparam>
    /// <returns>A set for the given entity type</returns>
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    /// <summary>
    /// Saves all changes made in this unit of work to the underlying database
    /// </summary>
    Task SaveChanges(CancellationToken cancellationToken = default);
}