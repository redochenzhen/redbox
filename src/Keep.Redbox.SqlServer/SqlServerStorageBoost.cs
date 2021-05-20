using Keep.Redbox.Storage;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Redbox.SqlServer
{
    public class SqlServerStorageBoost : IStorageBoost 
    {
        private const string TABLE_NAME = "CacheKey";
        private readonly ILogger _logger;
        private readonly SqlServerOptions _options;

        public string FullTableName => $"{_options.Schema}.{TABLE_NAME}";

        public SqlServerStorageBoost(
            ILogger<SqlServerStorageBoost> logger,
            IOptions<SqlServerOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            string schema = _options.Schema;
            var sql = $@"
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = '{schema}')
BEGIN
	EXEC('CREATE SCHEMA [{schema}]')
END;

IF OBJECT_ID(N'{FullTableName}',N'U') IS NULL
BEGIN
    CREATE TABLE {FullTableName}(
        [Id] [bigint] NOT NULL,
        [Version] [nvarchar](20) NOT NULL,
        [Key] [nvarchar](1024) NOT NULL,
        [Retries] [int] NOT NULL,
        [AddedAt] [datetime2](7) NOT NULL,
        [ExpiresAt] [datetime2](7) NULL,
        [State] [int] NOT NULL,
        CONSTRAINT [PK_{FullTableName}] PRIMARY KEY CLUSTERED
        (
	        [Id] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END;";
            using (var conn = new SqlConnection(_options.ConnectionString))
            {
                await conn.OpenAsync(cancellationToken);
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }
    }
}
