using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Processor;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Events.Inbox
{
    internal class InboxProcessorJobFactory : IJobFactory
    {
        private readonly Internals _internals;

        public InboxProcessorJobFactory(Internals internals)
        {
            _internals = internals;
        }

        [EntryPoint]
        public async Task<IJob?> TryCreateJobAsync(CancellationToken cancellationToken)
        {
            await using AsyncServiceScope scope = _internals.CreateAsyncScope();

            IInboxConsumer inbox = scope.ServiceProvider.GetRequiredService<IInboxConsumer>();
            InboxEvent? receivedEvent = (await inbox.Read(1, cancellationToken).ConfigureAwait(false)).FirstOrDefault();

            return receivedEvent == null
                ? null
                : new InboxProcessorJob(_internals).ForEvent(receivedEvent);
        }
    }
}
