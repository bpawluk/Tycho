using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Inbox
{
    internal interface IInboxConsumer
    {
        Task<IReadOnlyCollection<InboxEntry>> Read(int count, CancellationToken cancellationToken = default);

        Task MarkAsHandled(InboxEntry entry, CancellationToken cancellationToken = default);

        Task MarkAsFailed(InboxEntry entry, CancellationToken cancellationToken = default);
    }
}
