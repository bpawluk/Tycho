using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Events;

namespace Tycho.Persistence.EFCore;

public interface IUnitOfWork : IEventPublisher
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    Task SaveChanges(CancellationToken cancellationToken = default);
}