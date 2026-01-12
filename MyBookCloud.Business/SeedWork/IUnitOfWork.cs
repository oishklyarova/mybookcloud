using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBookCloud.Business.SeedWork
{
    public interface IUnitOfWork<out TContext> where TContext : new()
    {
        //The following Property is going to hold the context object
        TContext Context { get; }
        //Start the database Transaction
        void CreateTransaction();
        //Commit the database Transaction
        void Commit();
        //Rollback the database Transaction
        void Rollback();
        //DbContext Class SaveChanges method
        void SaveChanges(bool ignoreSoftDeleteFilter = false);
        Task<int> SaveChangesAsync(bool ignoreSoftDeleteFilter = false, CancellationToken cancellationToken = default);
    }
}
