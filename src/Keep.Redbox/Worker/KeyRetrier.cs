using Keep.Redbox.Cache;
using Keep.Redbox.Contract;
using Keep.Redbox.Pump;
using Keep.Redbox.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Redbox.Worker
{
    internal class KeyRetrier : IWorker
    {
        private const int RETRY_INTERVAL = 2000;

        private readonly ICacheClientFactory _clientFactory;
        private readonly IKeyStorage _keyStorage;
        private readonly ILogger<KeyRetrier> _logger;

        public KeyRetrier(
            ILogger<KeyRetrier> logger,
            ICacheClientFactory cacheClientFactory,
            IKeyStorage keyStorage)
        {
            _logger = logger;
            _clientFactory = cacheClientFactory;
            _keyStorage = keyStorage;
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            for (; ; )
            {
                var keys = default(IEnumerable<string>);
                try
                {
                    keys = await _keyStorage.GetCandidateKeysAsync();
                    using (var client = _clientFactory.CreateClient())
                    {
                        foreach (var key in keys)
                        {
                            await client.RemoveAsync(key, stoppingToken);
                            await _keyStorage.UpdateStateAsync(key, KeyState.Succeeded);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                await Task.Delay(RETRY_INTERVAL);
            }
        }

        public void Dispose()
        {
        }

        public void Stop()
        {
            Dispose();
        }
    }
}
