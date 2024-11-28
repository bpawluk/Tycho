using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Persistence
{
    internal interface IOutboxWriter
    {
        Task Write(
            IReadOnlyCollection<OutboxEntry> entries, 
            bool shouldCommit, 
            CancellationToken cancellationToken = default);
    }
}