using Keep.Redbox.Cache;
using ServiceStack.Redis;

namespace Keep.Redbox.Redis
{
    public class RedisClientFactory : ICacheClientFactory
    {
        private readonly IRedisClientsManager _manager;
        private readonly IRedisClientsManagerAsync _managerAsync;

        public RedisClientFactory(
            IRedisClientsManager redisClientsManager,
            IRedisClientsManagerAsync redisClientsManagerAsync)
        {
            _manager = redisClientsManager;
            _managerAsync = redisClientsManagerAsync;
        }

        public ICacheClient CreateClient()
        {
            var client = _manager.GetClient();
            var clientAsync = _managerAsync.GetClientAsync().Result;
            return new RedisClient(client, clientAsync);
        }
    }
}
