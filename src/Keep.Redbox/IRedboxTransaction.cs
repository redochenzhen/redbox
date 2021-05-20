using System;
using System;
using System.Data;

namespace Keep.Redbox
{
    public interface IRedboxTransaction : IDisposable
    {
        bool AutoCommit { get; set; }

        IDbTransaction DbTransaction { get; set; }

        event EventHandler Committed;

        event EventHandler Rollbacked;

        void Commit();

        void Rollback();
    }
}
