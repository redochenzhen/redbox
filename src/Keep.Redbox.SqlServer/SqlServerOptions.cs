namespace Keep.Redbox.SqlServer
{
    public class SqlServerOptions
    {
        public const string CONFIG_PREFIX = "redbox:sqlserver";

        public string ConnectionString { get; set; }

        public string Schema { get; set; } = "redbox";

        public bool IsSqlServer2008 { get; set; }
    }
}
