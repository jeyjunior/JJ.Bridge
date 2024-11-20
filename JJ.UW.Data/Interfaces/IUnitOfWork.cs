using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JJ.UW.Data.Interfaces
{
    public interface IUnitOfWork
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        void Begin();
        void Commit();
        void Rollback();
        void Dispose();
    }
}
