using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Redbox.Worker
{
    internal class KeyCleaner : IWorker
    {
        public void Dispose()
        {
      
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
       
        }

        public void Stop()
        {
            Dispose();
        }
    }
}
