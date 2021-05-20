using Keep.Redbox;
using Keep.Redbox.SqlServer;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedboxOptionsExts
    {
        public static RedboxOptions UseSqlServer(this RedboxOptions options, Action<SqlServerOptions> configure)
        {
            options.RegisterPlugin(new SqlServerPlugin(configure));
            return options;
        }

        public static RedboxOptions UseSqlServer(this RedboxOptions options)
        {
            options.RegisterPlugin(new SqlServerPlugin());
            return options;
        }
    }
}
