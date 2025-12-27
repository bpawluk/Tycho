using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Inbox
{
    internal interface IInboxConsumer
    {
        Task<IReadOnlyCollection<InboxEntry>> Read(int count, CancellationToken cancellationToken = default);

        Task MarkAsHandled(Guid entryId, CancellationToken cancellationToken = default);

        Task MarkAsFailed(Guid entryId, CancellationToken cancellationToken = default);
    }
}
