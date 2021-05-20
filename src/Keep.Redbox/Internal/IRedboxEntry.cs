using System.Threading;
using System.Threading.Tasks;

namespace Keep.Redbox.Internal
{
    internal interface IRedboxEntry
    {
        Task BootAsync(CancellationToken stoppingToken);
    }
}
