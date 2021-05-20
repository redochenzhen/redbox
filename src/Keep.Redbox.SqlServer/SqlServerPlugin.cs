using Keep.Framework;
using Keep.Redbox.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Keep.Redbox.SqlServer
{
    internal sealed class SqlServerPlugin : PluginBase<SqlServerOptions>
    {
        public SqlServerPlugin() { }

        public SqlServerPlugin(Action<SqlServerOptions> configure) : base(configure) { }

        protected override void Configure(IServiceCollection services, IConfiguration config)
        {
            services.Configure<SqlServerOptions>(options=>
            {
                options.ConnectionString = config.GetConnectionString("DefaultConnection");
            });
            services.Configure<SqlServerOptions>(config.GetSection(SqlServerOptions.CONFIG_PREFIX));
        }

        public override void Register(IServiceCollection services)
        {
            base.Register(services);

            services.AddSingleton<IKeyStorage, SqlServerKeyStorage>();
            services.AddSingleton<IStorageBoost , SqlServerStorageBoost>();
            services.AddSingleton<IRedboxTransaction, SqlServerTransaction>();
        }
    }
}
