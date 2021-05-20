using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Redbox.Storage
{
    public interface IStorageBoost 
    {
        string FullTableName { get; }

        Task InitializeAsync(CancellationToken cancellationToken);
    }
}
