using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Redbox.Cache
{
    internal class CacheProxy : ICacheProxy
    {
        public bool DeleteKey(string key)
        {
            throw new NotImplementedException();
        }

        public bool DeleteKey(string[] keys)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteKeyAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteKeyAsync(string[] keys)
        {
            throw new NotImplementedException();
        }
    }
}
