using Keep.Redbox.Storage;
using Keep.Redbox.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Redbox.Internal
{
    internal class RedboxEntry : BackgroundService, IRedboxEntry
    {
        private readonly ILogger _logger;
        private readonly IStorageBoost  _initializer;
        private readonly IEnumerable<IWorker> _workers;

        public RedboxEntry(
            ILogger<RedboxEntry> logger,
            IStorageBoost  initializer,
            IEnumerable<IWorker> workers)
        {
            _logger = logger;
            _initializer = initializer;
            _workers = workers;
        }

        public async Task BootAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("*** Redbox service is booting.");
            try
            {
                await _initializer.InitializeAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize the key storage.");
            }

            stoppingToken.Register(() =>
            {
                _logger.LogDebug("*** Redbox service is stopping.");
                foreach (var worker in _workers)
                {
                    worker.Stop();
                }
            });

            try
            {
                await Task.WhenAll(_workers.Select(async w => await w.StartAsync(stoppingToken)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BootAsync(stoppingToken);
        }
    }
}
