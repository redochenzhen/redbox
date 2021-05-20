using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Keep.Redbox
{
    public static class DbTransactionExts
    {
        public static IDbTransaction BeginTransaction(this IDbConnection dbConnection, IRedbox redbox, bool autoCommit = false)
        {
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }
            var dbTx = dbConnection.BeginTransaction();
            var tx = redbox.ServiceProvider.GetService<IRedboxTransaction>();
            tx.DbTransaction = dbTx;
            tx.AutoCommit = autoCommit;
            redbox.Transaction.Value = tx;
            return dbTx;
        }
    }
}
