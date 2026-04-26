using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;

namespace Tycho.Events.Inbox
{
    internal interface IInboxWriter
    {
        Task Write(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken = default);
    }
}
