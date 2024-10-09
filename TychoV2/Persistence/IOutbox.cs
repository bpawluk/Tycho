using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TychoV2.Persistence
{
    internal interface IOutbox
    {
        Task Add(IReadOnlyCollection<Entry> entries, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<Entry>> Read(int count, CancellationToken cancellationToken = default);

        Task MarkAsProcessed(Entry entry, CancellationToken cancellationToken = default);

        Task MarkAsFailed(Entry entry, CancellationToken cancellationToken = default);
    }
}
