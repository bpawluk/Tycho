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
        public async Task<IJob?> TryCreateJobAsync(CancellationToken cancellationToken)
        {
            await using AsyncServiceScope scope = _internals.CreateAsyncScope();

            IOutboxConsumer outbox = scope.ServiceProvider.GetRequiredService<IOutboxConsumer>();
            OutboxEvent? eventToDeliver = (await outbox.Read(1, cancellationToken).ConfigureAwait(false)).FirstOrDefault();

            return eventToDeliver == null
                ? null
                : new OutboxProcessorJob(_internals).ForEvent(eventToDeliver);
        }
    }
}
