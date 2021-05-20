using Keep.Redbox.Cache;
using Keep.Redbox.Pump;
using Keep.Redbox.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Redbox.Internal
{
    internal class Redbox : IRedbox
    {
        private readonly IDispatcher _dispatcher;
        private readonly IKeyStorage _keyStorage;
        private readonly ICacheClientFactory _cacheFactory;

        public Redbox(
            IServiceProvider serviceProvider,
            IDispatcher dispatcher,
            IKeyStorage keyStorage,
            ICacheClientFactory cacheFactory)
        {
            ServiceProvider = serviceProvider;
            _dispatcher = dispatcher;
            _keyStorage = keyStorage;
            _cacheFactory = cacheFactory;
            Transaction = new AsyncLocal<IRedboxTransaction>();
        }

        public IServiceProvider ServiceProvider { get; }

        public AsyncLocal<IRedboxTransaction> Transaction { get; }

        public T Get<T>(string key, Func<T> valueFunc)
        {
            T value;
            using (var client = _cacheFactory.CreateClient())
            {
                if (client.ContainsKey(key))
                {
                    value = client.Get<T>(key);
                    return value;
                }
            }
            value = valueFunc();
            using (var client = _cacheFactory.CreateClient())
            {
                client.Set(key, value);
            }
            return value;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> valueFuncAsync)
        {
            T value;
#if NETSTANDARD2_1
            await
#endif
            using (var client = _cacheFactory.CreateClient())
            {
                if (await client.ContainsKeyAsync(key))
                {
                    value = await client.GetAsync<T>(key);
                    return value;
                }
            }
            value = await valueFuncAsync();
#if NETSTANDARD2_1
            await
#endif
            using (var client = _cacheFactory.CreateClient())
            {
                await client.SetAsync(key, value);
            }
            return value;
        }

        public void Remove(params string[] keys)
        {
            keys = keys ?? throw new ArgumentNullException(nameof(keys));
            var tx = Transaction.Value;
            if (tx == null)
            {
                _keyStorage.StoreKeys(keys);
                _dispatcher.DispatchToRemove(keys);
            }
            else
            {
                _keyStorage.StoreKeys(keys, tx.DbTransaction);
                tx.Committed += (s, e) => _dispatcher.DispatchToRemove(keys);
                if (tx.AutoCommit)
                {
                    tx.Commit();
                }
            }
        }
    }
}
