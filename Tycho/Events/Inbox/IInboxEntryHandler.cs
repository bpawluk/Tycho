using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Inbox
{
    internal interface IInboxEntryHandler
    {
        Task<bool> TryHandlingEntryAsync(InboxEntry entry, CancellationToken cancellationToken);
    }
}
