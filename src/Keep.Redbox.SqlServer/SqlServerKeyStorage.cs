using Keep.Redbox.Contract;
using Keep.Redbox.Storage;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Keep.Redbox.SqlServer
{
    internal class SqlServerKeyStorage : IKeyStorage
    {
        public const string FIELD_LIST = "[Id],[Version],[Key],[Retries],[AddedAt],[ExpiresAt],[State]";
        public const string FIELD_PARAM_LIST = "@Id,@Version,@Key,@Retries,@AddedAt,@ExpiresAt,@State";

        private readonly SqlServerOptions _options;
        private readonly RedboxOptions _redboxOptions;
        private readonly IStorageBoost _boost;

        public SqlServerKeyStorage(
            IOptions<SqlServerOptions> options,
            IOptions<RedboxOptions> redboxOptions,
            IStorageBoost storageBoost)
        {
            _options = options.Value;
            _redboxOptions = redboxOptions.Value;
            _boost = storageBoost;
        }

        public async Task<List<RetryContext>> GetCandidateKeysAsync()
        {
            string sql = $@"
SELECT TOP ({_redboxOptions.BatchCount}) [Id],[Key],[Retries]
FROM {_boost.FullTableName} WITH (readpast) WHERE
[Version] = '{_redboxOptions.Version}' AND
[Retries] < {_redboxOptions.MaxFailedRetries} AND
([State] = {(int)KeyState.Failed} OR [State] = {(int)KeyState.Waiting})";

            List<RetryContext> ctxLst;
            using (var conn = new SqlConnection(_options.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    var reader = await cmd.ExecuteReaderAsync();
                    ctxLst = new List<RetryContext>();
                    while (await reader.ReadAsync())
                    {
                        ctxLst.Add(new RetryContext
                        {
                            KeyId = (long)reader["Id"],
                            Key = (string)reader["Key"],
                            Retries = (int)reader["Retries"]
                        });
                    }
                    return ctxLst;
                }
            }
        }

        public void StoreKey(string key, IDbTransaction dbTransaction = default)
        {
            StoreKeys(new[] { key }, dbTransaction);
        }

        public void StoreKeys(IEnumerable<string> keys, IDbTransaction dbTransaction = default)
        {
            keys = keys ?? throw new ArgumentNullException(nameof(keys));

            string sql = $@"
INSERT INTO {_boost.FullTableName} ({FIELD_LIST}) VALUES({FIELD_PARAM_LIST});";
            Action<IDbConnection, IDbTransaction> exec = (conn, tx) =>
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                foreach (var key in keys)
                {
                    var paramLst = new List<SqlParameter>
                    {
                        new SqlParameter("@Id", Snowflake.Default().NextId()),
                        new SqlParameter("@Version", _redboxOptions.Version),
                        new SqlParameter("@Key", key),
                        new SqlParameter("@Retries", "0"),
                        new SqlParameter("@AddedAt", DateTime.Now),
                        new SqlParameter("@ExpiresAt", DBNull.Value),
                        new SqlParameter("@State", KeyState.Waiting)
                    };
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        paramLst.ForEach(p => cmd.Parameters.Add(p));
                        cmd.Transaction = dbTransaction;
                        cmd.ExecuteNonQuery();
                    }
                }
            };
            if (dbTransaction == null)
            {
                using (var conn = new SqlConnection(_options.ConnectionString))
                {
                    exec(conn, null);
                }
            }
            else
            {
                var conn = dbTransaction.Connection;
                exec(conn, dbTransaction);
            }
        }

        public async Task<bool> UpdateKeyAsync(long id, KeyState state, int retries)
        {
            //TODO: use Id
            string sql = $@"
UPDATE {_boost.FullTableName} SET
    [State] = {(int)state},
    [Retries] = {retries}
WHERE [Id] = {id}";

            using (var conn = new SqlConnection(_options.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    var n = await cmd.ExecuteNonQueryAsync();
                    return n == 1;
                }
            }
        }
    }
}
