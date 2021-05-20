using Keep.Redbox.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Redbox.Storage
{
    public interface IKeyStorage
    {
        void StoreKey(string key, IDbTransaction dbTransaction = default);

        void StoreKeys(IEnumerable<string> keys, IDbTransaction dbTransaction = default);

        Task<List<RetryContext>> GetCandidateKeysAsync();

        Task<bool> UpdateKeyAsync(long id, KeyState state, int retries);
    }
}
