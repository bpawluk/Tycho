using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Processor;

namespace Tycho.Events.Inbox
{
    internal class InboxProcessorJob : IJob
    {
        private readonly IInboxConsumer _inboxConsumer;
        private readonly IInboxEntryHandler _entryHandler;
        private readonly InboxProcessorSettings _settings;
        private readonly ILogger<InboxProcessorJob> _logger;

        private readonly List<Task> _entriesInProcessing = new List<Task>();

        public InboxProcessorJob(
            IInboxConsumer inboxConsumer,
            IInboxEntryHandler entryHandler,
            InboxProcessorSettings? settings = null,
            ILogger<InboxProcessorJob>? logger = null)
        {
            _inboxConsumer = inboxConsumer;
            _entryHandler = entryHandler;
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

        private async Task HandleEntryAsync(InboxEntry entry, CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Handler timeout support
                await _entryHandler.HandleEntryAsync(entry, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process inbox entry with ID {entryId}", entry.Id);
                await _inboxConsumer.MarkAsFailed(entry.Id, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
