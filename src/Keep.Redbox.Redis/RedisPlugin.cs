using Keep.Framework;
using Keep.Redbox.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ServiceStack.Redis;
using System;

namespace Keep.Redbox.Redis
{
    internal sealed class RedisPlugin : PluginBase<RedisOptions>
    {
        public RedisPlugin() { }

        public RedisPlugin(Action<RedisOptions> configure) : base(configure) { }

        protected override void Configure(IServiceCollection services, IConfiguration config)
        {
            services.Configure<RedisOptions>(options =>
            {
                options.ConnectionString = config.GetConnectionString(RedisOptions.CONNECTION_NAME);
            });
            services.Configure<RedisOptions>(config.GetSection(RedisOptions.CONFIG_PREFIX));
        }

        public override void Register(IServiceCollection services)
        {
            base.Register(services);

            //services.AddSingleton<ICacheClient, RedisClient>();
            services.AddSingleton<ICacheClientFactory, RedisClientFactory>();
            var sp = services.BuildServiceProvider();
            var options = sp.GetService<IOptions<RedisOptions>>().Value;
            services.AddSingleton<IRedisClientsManager>(_ => new RedisManagerPool(options.ConnectionString));
            services.AddSingleton<IRedisClientsManagerAsync>(_ => new RedisManagerPool(options.ConnectionString));
        }
    }
}
