using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Redbox.Worker
{
    public interface IWorker : IDisposable
    {
        Task StartAsync(CancellationToken stoppingToken);

        void Stop();
    }
}
