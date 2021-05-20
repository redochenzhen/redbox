using Keep.Redbox.Cache;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SSR = ServiceStack.Redis;

namespace Keep.Redbox.Redis
{
    internal class RedisClient : ICacheClient, IDisposable, IAsyncDisposable
    {
        private readonly SSR.IRedisClient _client;
        private readonly SSR.IRedisClientAsync _clientAsync;


        public RedisClient(SSR.IRedisClient redisClient, SSR.IRedisClientAsync redisClientAsync)
        {
            _client = redisClient;
            _clientAsync = redisClientAsync;
        }

        public bool Add<T>(string key, T value, DateTime expiresAt)
        {
            return _client.Add(key, value, expiresAt);
        }

        public bool Add<T>(string key, T value)
        {
            return _client.Add(key, value);
        }

        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            return _client.Add(key, value, expiresIn);
        }

        public Task<bool> AddAsync<T>(string key, T value, CancellationToken token = default)
        {
            return _clientAsync.AddAsync(key, value, token);
        }

        public Task<bool> AddAsync<T>(string key, T value, DateTime expiresAt, CancellationToken token = default)
        {
            return _clientAsync.AddAsync(key, value, expiresAt, token);
        }

        public Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public long Decrement(string key, uint amount)
        {
            throw new NotImplementedException();
        }

        public Task<long> DecrementAsync(string key, uint amount, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _client?.Dispose();
            _clientAsync.DisposeAsync().AsTask().Wait();
        }

        public async ValueTask DisposeAsync()
        {
            _client?.Dispose();
            await _clientAsync.DisposeAsync();
        }

        public void FlushAll()
        {
            throw new NotImplementedException();
        }

        public Task FlushAllAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }

        public Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(string key, CancellationToken token = default)
        {
            return _clientAsync.GetAsync<T>(key, token);
        }

        public Task<TimeSpan?> GetTimeToLiveAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public long Increment(string key, uint amount)
        {
            throw new NotImplementedException();
        }

        public Task<long> IncrementAsync(string key, uint amount, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            return _client.Remove(key);
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            _client.RemoveAll(keys);
        }

        public Task RemoveAllAsync(IEnumerable<string> keys, CancellationToken token = default)
        {
            return _clientAsync.RemoveAllAsync(keys, token);
        }

        public Task<bool> RemoveAsync(string key, CancellationToken token = default)
        {
            return _clientAsync.RemoveAsync(key, token);
        }

        public Task RemoveExpiredEntriesAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public bool Replace<T>(string key, T value, DateTime expiresAt)
        {
            throw new NotImplementedException();
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            throw new NotImplementedException();
        }

        public bool Replace<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceAsync<T>(string key, T value, DateTime expiresAt, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceAsync<T>(string key, T value, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            throw new NotImplementedException();
        }

        public bool Set<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            throw new NotImplementedException();
        }

        public void SetAll<T>(IDictionary<string, T> values)
        {
            throw new NotImplementedException();
        }

        public Task SetAllAsync<T>(IDictionary<string, T> values, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetAsync<T>(string key, T value, CancellationToken token = default)
        {
            return _clientAsync.SetAsync(key, value, token);
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan expiresIn, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetAsync<T>(string key, T value, DateTime expiresAt, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            return _client.ContainsKey(key);
        }

        public ValueTask<bool> ContainsKeyAsync(string key, CancellationToken token = default)
        {
            return _clientAsync.ContainsKeyAsync(key, token);
        }
    }
}
