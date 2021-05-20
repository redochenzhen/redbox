using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Redbox.Cache
{
    internal interface ICacheProxy
    {
        bool DeleteKey(string key);

        Task<bool> DeleteKeyAsync(string key);

        bool DeleteKey(string[] keys);

        Task<bool> DeleteKeyAsync(string[] keys);
    }
}
