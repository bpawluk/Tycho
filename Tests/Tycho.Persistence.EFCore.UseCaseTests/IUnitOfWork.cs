using Microsoft.EntityFrameworkCore;

namespace Tycho.UseCaseTests;

internal interface IUnitOfWork
{
    DbSet<T> Set<T>() where T : class;

    Task SaveChanges(CancellationToken cancellationToken);

    Task Publish(object any, CancellationToken cancellationToken);
}
