using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Redbox.Cache
{
    public interface ICacheClient : IDisposable, IAsyncDisposable
    {
        bool Add<T>(string key, T value, DateTime expiresAt);
        bool Add<T>(string key, T value);
        bool Add<T>(string key, T value, TimeSpan expiresIn);
        long Decrement(string key, uint amount);
        void FlushAll();
        T Get<T>(string key);
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);
        long Increment(string key, uint amount);
        bool Remove(string key);
        void RemoveAll(IEnumerable<string> keys);
        bool Replace<T>(string key, T value, DateTime expiresAt);
        bool Replace<T>(string key, T value, TimeSpan expiresIn);
        bool Replace<T>(string key, T value);
        bool Set<T>(string key, T value, DateTime expiresAt);
        bool Set<T>(string key, T value);
        bool Set<T>(string key, T value, TimeSpan expiresIn);
        void SetAll<T>(IDictionary<string, T> values);
        Task<bool> AddAsync<T>(string key, T value, CancellationToken token = default);
        Task<bool> AddAsync<T>(string key, T value, DateTime expiresAt, CancellationToken token = default);
        Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn, CancellationToken token = default);
        Task<long> DecrementAsync(string key, uint amount, CancellationToken token = default);
        Task FlushAllAsync(CancellationToken token = default);
        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys, CancellationToken token = default);
        Task<T> GetAsync<T>(string key, CancellationToken token = default);
        //IAsyncEnumerable<string> GetKeysByPatternAsync(string pattern, CancellationToken token = default);
        Task<TimeSpan?> GetTimeToLiveAsync(string key, CancellationToken token = default);
        Task<long> IncrementAsync(string key, uint amount, CancellationToken token = default);
        Task RemoveAllAsync(IEnumerable<string> keys, CancellationToken token = default);
        Task<bool> RemoveAsync(string key, CancellationToken token = default);
        Task RemoveExpiredEntriesAsync(CancellationToken token = default);
        Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn, CancellationToken token = default);
        Task<bool> ReplaceAsync<T>(string key, T value, DateTime expiresAt, CancellationToken token = default);
        Task<bool> ReplaceAsync<T>(string key, T value, CancellationToken token = default);
        Task SetAllAsync<T>(IDictionary<string, T> values, CancellationToken token = default);
        Task<bool> SetAsync<T>(string key, T value, CancellationToken token = default);
        Task<bool> SetAsync<T>(string key, T value, TimeSpan expiresIn, CancellationToken token = default);
        Task<bool> SetAsync<T>(string key, T value, DateTime expiresAt, CancellationToken token = default);
        bool ContainsKey(string key);
        ValueTask<bool> ContainsKeyAsync(string key, CancellationToken token = default);
    }
}
