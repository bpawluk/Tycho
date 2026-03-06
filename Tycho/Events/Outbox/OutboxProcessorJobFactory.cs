using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Processor;
using Tycho.Structure;

namespace Tycho.Events.Outbox
{
    internal class OutboxProcessorJobFactory : IJobFactory
    {
        private readonly Internals _internals;
        private readonly IOutboxConsumer _outbox;

        public OutboxProcessorJobFactory(Internals internals, IOutboxConsumer outbox)
        {
            _internals = internals;
            _outbox = outbox;
        }

        public async Task<IReadOnlyCollection<IJob>> CreateJobsAsync(int maxCount, CancellationToken cancellationToken)
        {
            var eventsToDeliver = await _outbox.Read(maxCount, cancellationToken).ConfigureAwait(false);
            return eventsToDeliver
                .Select(eventToDeliver => _internals
                    .GetRequiredService<OutboxProcessorJob>()
                    .ForEvent(eventToDeliver))
                .ToArray();
        }
    }
}