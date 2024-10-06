using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TychoV2.Events.Broker;

namespace TychoV2.Persistence
{
    internal class OutboxProcessor : IDisposable
    {
        private readonly IEventProcessor _eventProcessor;
        private readonly IOutbox _eventOutbox;
        private readonly OutboxProcessorSettings _settings;

        private readonly Timer _timer;
        private readonly SemaphoreSlim _semaphore;

        private TimeSpan _currentPollingInterval = Timeout.InfiniteTimeSpan;

        public OutboxProcessor(
            IOutbox eventOutbox, 
            IEventProcessor eventProcessor, 
            OutboxProcessorSettings settings)
        {
            _settings = settings;
            _eventProcessor = eventProcessor;
            _eventOutbox = eventOutbox;

            _semaphore = new SemaphoreSlim(1, 1);
            _timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        public async Task StartPolling()
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                SetIntervalUnsafe(_settings.InitialPollingInterval);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async void TimerCallback(object? _)
        {
            try
            {
                await ProcessOutbox().ConfigureAwait(false);
            }
            catch
            {
                // TODO: Log the Exception
            }
        }

        private async Task ProcessOutbox()
        {
            var nextEntries = await GetNextEntries().ConfigureAwait(false);
            var processEntryTasks = nextEntries.Select(async entry =>
            {
                var succesfullyProcessed = await _eventProcessor
                    .Process(entry.HandlerId, entry.Payload)
                    .ConfigureAwait(false);

                if (succesfullyProcessed)
                {
                    await _eventOutbox.MarkAsProcessed(entry).ConfigureAwait(false);
                }
                else
                {
                    await _eventOutbox.MarkAsFailed(entry).ConfigureAwait(false);
                }
            });
            await Task.WhenAll(processEntryTasks).ConfigureAwait(false);
        }

        private async Task<IReadOnlyCollection<Entry>> GetNextEntries()
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                var nextEntries = await _eventOutbox.Read(_settings.BatchSize).ConfigureAwait(false);

                if (nextEntries.Count == 0)
                {
                    // Increase polling
                    SetIntervalUnsafe(_currentPollingInterval * _settings.PollingIntervalMultiplier);
                }

                return nextEntries;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void SetIntervalUnsafe(TimeSpan interval)
        {
            if (interval == _currentPollingInterval)
            {
                return;
            }

            if (interval > _settings.MaxPollingInterval)
            {
                // Stop polling
                interval = Timeout.InfiniteTimeSpan;
            }

            _currentPollingInterval = interval;
            _timer.Change(_currentPollingInterval, _currentPollingInterval);
        }

        public void Dispose()
        {
            _semaphore.Dispose();
            _timer.Dispose();
        }
    }
}