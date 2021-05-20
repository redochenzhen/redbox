using System;
using System.Data;

namespace Keep.Redbox.SqlServer
{
    public class SqlServerTransaction : IRedboxTransaction
    {
        public bool AutoCommit { get; set; }
        public IDbTransaction DbTransaction { get; set; }

        public event EventHandler Committed;

        public event EventHandler Rollbacked;

        public void Commit()
        {
            if (DbTransaction == null) return;
            DbTransaction.Commit();
            //throw new Exception("dummy crash");
            OnCommitted();
        }

        public void Dispose()
        {
            DbTransaction?.Dispose();
        }

        public void Rollback()
        {
            if (DbTransaction == null) return;
            DbTransaction.Rollback();
            OnRollbacked();
        }

        protected virtual void OnCommitted()
        {
            if (Committed != null)
            {
                Committed(this, new EventArgs());
            }
        }

        protected virtual void OnRollbacked()
        {
            if (Rollbacked != null)
            {
                Rollbacked(this, new EventArgs());
            }
        }
    }
}
