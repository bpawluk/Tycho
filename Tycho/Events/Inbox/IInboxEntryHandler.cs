using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Inbox
{
    internal interface IInboxEntryHandler
    {
        Task HandleEntryAsync(InboxEntry entry, CancellationToken cancellationToken);
    }
}
