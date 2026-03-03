using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;

namespace Tycho.Events.Inbox
{
    internal interface IInboxWriter
    {
        Task Write(RoutedEvent entry, CancellationToken cancellationToken = default);
    }
}
