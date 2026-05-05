using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Processor;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Events.Outbox
{
    internal class OutboxProcessorJobFactory : IJobFactory
    {
        private readonly Internals _internals;

        public OutboxProcessorJobFactory(Internals internals)
        {
            _internals = internals;
        }

        [EntryPoint]
        public async Task<IReadOnlyCollection<IJob>> CreateJobsAsync(int maxCount, CancellationToken cancellationToken)
        {
            await using var scope = _internals.CreateAsyncScope();

            var outbox = scope.ServiceProvider.GetRequiredService<IOutboxConsumer>();
            var eventsToDeliver = await outbox.Read(maxCount, cancellationToken).ConfigureAwait(false);

            return eventsToDeliver.Select(eventToDeliver => new OutboxProcessorJob(_internals).ForEvent(eventToDeliver)).ToArray();
        }
    }
}