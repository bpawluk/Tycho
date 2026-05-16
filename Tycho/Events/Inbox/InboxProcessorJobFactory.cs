using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Model;
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
        public async Task<IReadOnlyCollection<IJob>> CreateJobsAsync(int maxCount, CancellationToken cancellationToken)
        {
            await using AsyncServiceScope scope = _internals.CreateAsyncScope();

            IInboxConsumer inbox = scope.ServiceProvider.GetRequiredService<IInboxConsumer>();
            IReadOnlyCollection<RoutedEvent> receivedEvents = await inbox.Read(maxCount, cancellationToken).ConfigureAwait(false);

            return receivedEvents.Select(receivedEvent => new InboxProcessorJob(_internals).ForEvent(receivedEvent)).ToArray();
        }
    }
}
