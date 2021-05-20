using Keep.Redbox.Pump;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Redbox.Worker
{
    internal class KeyPostman : IWorker
    {
        private readonly ILogger _logger;
        private readonly IDispatcher _dispatcher;

        public KeyPostman(
            ILogger<KeyPostman> logger,
            IDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Redbox postman is starting.");
            _dispatcher.Pumping();
            return Task.CompletedTask;
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
