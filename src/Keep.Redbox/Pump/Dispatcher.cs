using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Keep.Redbox.Cache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Keep.Redbox.Pump
{
    internal class Dispatcher : IDispatcher
    {
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _cts;
        private readonly Channel<IEnumerable<string>> _keysBuffer;
        private readonly RedboxOptions _options;
        private readonly ICacheClientFactory _cacheFactory;

        public Dispatcher(
            ILogger<Dispatcher> logger,
            IOptions<RedboxOptions> options,
            ICacheClientFactory cacheFactory)
        {
            _cts = new CancellationTokenSource();
            _keysBuffer = Channel.CreateUnbounded<IEnumerable<string>>();
            _logger = logger;
            _options = options.Value;
            _cacheFactory = cacheFactory;
        }

        public bool DispatchToRemove(IEnumerable<string> keys)
        {
            var writer = _keysBuffer.Writer;
            return writer.TryWrite(keys);
        }

        public async Task<bool> DispatchToRemoveAsync(IEnumerable<string> keys)
        {
            var writer = _keysBuffer.Writer;
            try
            {
                await writer.WriteAsync(keys, _cts.Token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public void Pumping()
        {
            LongRun(Deleting);
        }

        private void Deleting()
        {
            var reader = _keysBuffer.Reader;
            while (reader.WaitToReadAsync(_cts.Token).AsTask().Result)
            {
                while (reader.TryRead(out var keys))
                {
                    using (var cache = _cacheFactory.CreateClient())
                    {
                        cache.RemoveAll(keys);
                    }
                }
            }
        }

        private void LongRun(Action action)
        {
            Task.Factory.StartNew(action, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
