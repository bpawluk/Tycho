using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Events.Dispatching;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Processor;

namespace Tycho.Events.Inbox
{
    internal class InboxProcessorJob : IJob
    {
        private readonly IInboxConsumer _inboxConsumer;
        private readonly IEventHandlerProvider _handlerRegistry;
        private readonly IEventDispatcher _handlingDispatcher;
        private readonly InboxProcessorSettings _settings;
        private readonly ILogger<InboxProcessorJob> _logger;

        private readonly List<Task> _entriesInProcessing = new List<Task>();

        public InboxProcessorJob(
            IInboxConsumer inboxConsumer,
            IEventHandlerProvider handlerRegistry,
            IEventDispatcher handlingDispatcher,
            InboxProcessorSettings? settings = null,
            ILogger<InboxProcessorJob>? logger = null)
        {
            _inboxConsumer = inboxConsumer;
            _handlerRegistry = handlerRegistry;
            _handlingDispatcher = handlingDispatcher;
            _settings = settings ?? InboxProcessorSettings.Default;
            _logger = logger ?? NullLogger<InboxProcessorJob>.Instance;
        }

        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            _entriesInProcessing.RemoveAll(t => t.IsCompleted);

            var newEntriesCount = 0;
            var entriesInProcessingCount = _entriesInProcessing.Count;

            var entriesToFetch = Math.Min(_settings.ConcurrencyLimit - entriesInProcessingCount, _settings.BatchSize);
            if (entriesToFetch > 0)
            {
                var newEntries = await _inboxConsumer.Read(entriesToFetch, cancellationToken).ConfigureAwait(false);
                newEntriesCount = newEntries.Count;

                var newEntriesInProcessing = newEntries.Select(entry => HandleEntryAsync(entry, cancellationToken));
                _entriesInProcessing.AddRange(newEntriesInProcessing);
            }

            return newEntriesCount > 0 || entriesInProcessingCount > 0;
        }

        private async Task HandleEntryAsync(RoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Handler timeout support
                await routedEvent.DispatchAsync(_handlingDispatcher, cancellationToken).ConfigureAwait(false);
                await _inboxConsumer.MarkAsHandled(routedEvent.Id, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process inbox entry with ID {entryId}", routedEvent.Id);
                await _inboxConsumer.MarkAsFailed(routedEvent.Id, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
