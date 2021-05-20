using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Redbox.Pump
{
    internal interface IDispatcher : IDisposable
    {
        void Pumping();

        bool DispatchToRemove(IEnumerable<string> keys);

        Task<bool> DispatchToRemoveAsync(IEnumerable<string> keys);
    }
}
