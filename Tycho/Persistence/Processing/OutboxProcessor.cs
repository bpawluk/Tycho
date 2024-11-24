﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tycho.Persistence.Processing
{
    internal class OutboxProcessor : IDisposable
    {
        private readonly IOutboxConsumer _eventOutbox;
        private readonly OutboxActivity _outboxActivity;
        private readonly IEntryProcessor _entryProcessor;
        private readonly OutboxProcessorSettings _settings;
        private readonly ILogger<OutboxProcessor> _logger;

        private readonly SemaphoreSlim _processingSemaphore;
        private readonly object _timerChangeLock;
        private readonly Timer _timer;

        private readonly List<Task> _entriesInProcessing = new List<Task>();

        private TimeSpan _currentPollingInterval = Timeout.InfiniteTimeSpan;

        public OutboxProcessor(
            IOutboxConsumer eventOutbox,
            OutboxActivity outboxActivity,
            IEntryProcessor entryProcessor,
            OutboxProcessorSettings settings,
            ILogger<OutboxProcessor>? logger = null)
        {
            _eventOutbox = eventOutbox;
            _outboxActivity = outboxActivity;
            _entryProcessor = entryProcessor;
            _settings = settings;
            _logger = logger ?? NullLogger<OutboxProcessor>.Instance;

            _timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);

            _timerChangeLock = new object();
            _processingSemaphore = new SemaphoreSlim(1, 1);
        }

        public void Initialize()
        {
            _outboxActivity.NewEntriesAdded += OnEntriesAdded;
        }

        private void OnEntriesAdded(object _, EventArgs __)
        {
            ResetInterval();
        }

        private async void TimerCallback(object? _)
        {
            // Prevents overlapping processing
            if (await _processingSemaphore.WaitAsync(0).ConfigureAwait(false))
            {
                try
                {
                    await ProcessOutbox().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Outbox processing failed");
                }
                finally
                {
                    _processingSemaphore.Release();
                }
            }
        }

        private async Task ProcessOutbox()
        {
            _entriesInProcessing.RemoveAll(t => t.IsCompleted);

            var newEntriesCount = 0;
            var entriesInProcessingCount = _entriesInProcessing.Count;

            var entriesToFetch = Math.Min(_settings.ConcurrencyLimit - entriesInProcessingCount, _settings.BatchSize);
            if (entriesToFetch > 0)
            {
                var newEntries = await _eventOutbox.Read(entriesToFetch).ConfigureAwait(false);
                newEntriesCount = newEntries.Count;

                var newEntriesInProcessing = newEntries.Select(ProcessEntry);
                _entriesInProcessing.AddRange(newEntriesInProcessing);
            }

            if (newEntriesCount == 0 && entriesInProcessingCount == 0)
            {
                IncreaseInterval(); // When processor is idle
            }
        }

        private async Task ProcessEntry(OutboxEntry entry)
        {
            try
            {
                var isProcessed = await _entryProcessor.Process(entry).ConfigureAwait(false);
                if (isProcessed)
                {
                    await _eventOutbox.MarkAsProcessed(entry).ConfigureAwait(false);
                }
                else
                {
                    await _eventOutbox.MarkAsFailed(entry).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Entry processing failed ({entryId})", entry.Id);
            }
        }

        private void ResetInterval()
        {
            lock (_timerChangeLock)
            {
                if (_currentPollingInterval != _settings.InitialPollingInterval)
                {
                    _currentPollingInterval = _settings.InitialPollingInterval;
                    _timer.Change(TimeSpan.Zero, _currentPollingInterval);
                }
            }
        }

        private void IncreaseInterval()
        {
            lock (_timerChangeLock)
            {
                var newInterval = _currentPollingInterval * _settings.PollingIntervalMultiplier;

                if (newInterval > _settings.MaxPollingInterval)
                {
                    // Stop polling
                    newInterval = Timeout.InfiniteTimeSpan;
                }

                _currentPollingInterval = newInterval;
                _timer.Change(_currentPollingInterval, _currentPollingInterval);
            }
        }

        public void Dispose()
        {
            _outboxActivity.NewEntriesAdded -= OnEntriesAdded;
            _timer.Dispose();
            _processingSemaphore.Dispose();
        }
    }
}