using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Processor;
using Tycho.Structure;

namespace Tycho.Events.Inbox
{
    internal class InboxProcessorJobFactory : IJobFactory
    {
        private readonly Internals _internals;
        private readonly IInboxConsumer _inbox;

        public InboxProcessorJobFactory(Internals internals, IInboxConsumer inbox)
        {
            _internals = internals;
            _inbox = inbox;
        }

        public async Task<IReadOnlyCollection<IJob>> CreateJobsAsync(int maxCount, CancellationToken cancellationToken)
        {
            var receivedEvents = await _inbox.Read(maxCount, cancellationToken).ConfigureAwait(false);
            return receivedEvents
                .Select(receivedEvent => _internals
                    .GetRequiredService<InboxProcessorJob>()
                    .ForEvent(receivedEvent))
                .ToArray();
        }
    }
}
