using Keep.Redbox;
using Keep.Redbox.Redis;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedboxOptionsExts
    {
        public static RedboxOptions UseRedis(this RedboxOptions options, Action<RedisOptions> configure)
        {
            options.RegisterPlugin(new RedisPlugin(configure));
            return options;
        }

        public static RedboxOptions UseRedis(this RedboxOptions options)
        {
            options.RegisterPlugin(new RedisPlugin());
            return options;
        }
    }
}
