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
            while (!stoppingToken.IsCancellationRequested)
            {
                var ctxLst = default(IEnumerable<RetryContext>);
                try
                {
                    ctxLst = await _keyStorage.GetCandidateKeysAsync();
                    using (var client = _clientFactory.CreateClient())
                    {
                        foreach (var ctx in ctxLst)
                        {
                            bool? exists = default;
                            try
                            {
                                exists = await client.RemoveAsync(ctx.Key, stoppingToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Failedt to removing key: [{ctx.Key}].");
                                await _keyStorage.UpdateKeyAsync(ctx.KeyId, KeyState.Failed, ctx.Retries + 1);
                            }
                            if (exists.HasValue)
                            {
                                if (exists.Value)
                                {
                                    _logger.LogDebug($"Key: [{ctx.Key}] exists and has been removed.");
                                }
                                else
                                {
                                    _logger.LogDebug($"Key: [{ctx.Key}] does not exist. No need to retry.");
                                }
                                await _keyStorage.UpdateKeyAsync(ctx.KeyId, KeyState.Succeeded, ctx.Retries);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                await Task.Delay(RETRY_INTERVAL, stoppingToken);
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
