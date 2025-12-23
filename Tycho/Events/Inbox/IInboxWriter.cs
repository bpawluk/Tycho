using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Inbox
{
    internal interface IInboxWriter
    {
        Task Write(InboxEntry entry, CancellationToken cancellationToken = default);
    }
}
