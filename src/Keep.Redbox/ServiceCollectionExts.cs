using Keep.Redbox;
using Keep.Redbox.Cache;
using Keep.Redbox.Internal;
using Keep.Redbox.Pump;
using Keep.Redbox.Worker;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExts
    {
        public static void AddRedbox(this IServiceCollection services, Action<RedboxOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            services.AddSingleton<IRedbox, Redbox>();
            services.AddSingleton<IDispatcher, Dispatcher>();
            services.AddSingleton<ICacheProxy, CacheProxy>();

            services.TryAddEnumerable(
                new[]
                {
                    ServiceDescriptor.Singleton<IWorker,KeyPostman>(),
                    ServiceDescriptor.Singleton<IWorker,KeyRetrier>(),
                    ServiceDescriptor.Singleton<IWorker,KeyCleaner>()
                });

            var options = new RedboxOptions();
            configure(options);
            options.Plugins.ForEach(plugin => plugin.Register(services));
            services.Configure(configure);

            services.AddSingleton<IRedboxEntry, RedboxEntry>();
            services.AddHostedService<RedboxEntry>();
        }
    }
}

namespace Keep.Redbox
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ServiceCollectionExts
    {
        /// <summary>
        /// 启动Redbox
        /// </summary>
        /// <param name="services">DI容器</param>
        public static async Task BootBus(this IServiceCollection services, CancellationToken stoppingToken = default)
        {
            var sp = services.BuildServiceProvider();
            var entry = sp.GetService<IRedboxEntry>();
            await entry?.BootAsync(stoppingToken);
        }
    }
}
