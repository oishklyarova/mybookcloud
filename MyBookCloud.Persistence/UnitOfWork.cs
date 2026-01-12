using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyBookCloud.Business.SeedWork;

namespace MyBookCloud.Persistence
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>, IDisposable where TContext : DbContext, new()
    {
        private bool _disposed;
        private string _errorMessage = string.Empty;
        //The following Object is going to hold the Transaction Object
        private IDbContextTransaction? activeTransaction;
        private Stack<Guid> transactions;
        //The Context property will return the DBContext object
        //This Property is declared inside the Parent Interface and Initialized through the Constructor
        public TContext Context { get; }
        public UnitOfWork(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            transactions = new Stack<Guid>();
        }

        //The Dispose() method is used to free unmanaged resources like files, 
        //database connections etc. at any time.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //The CreateTransaction() method will create a database Transaction so that we can do database operations
        //by applying do everything and do nothing principle
        public void CreateTransaction()
        {
            if (this.activeTransaction == null)
            {
                //It will Begin the transaction on the underlying store connection
                this.activeTransaction = this.Context.Database.BeginTransaction();
                this.transactions.Push(activeTransaction.TransactionId);
            }
            else
            {
                this.transactions.Push(Guid.NewGuid());
            }
        }
        //If all the Transactions are completed successfully then we need to call this Commit() 
        //method to Save the changes permanently in the database
        public void Commit()
        {
            if (activeTransaction == null)
            {
                return;
            }

            if (this.transactions.Count == 1 && this.transactions.First() == activeTransaction.TransactionId)
            {
                try
                {
                    //Commits the underlying store transaction
                    activeTransaction.Commit();
                }
                finally
                {
                    this.DisposeTransaction();
                }
            }
            else
            {
                this.transactions.Pop();
            }
        }
        //If at least one of the Transaction is Failed then we need to call this Rollback() 
        //method to Rollback the database changes to its previous state
        public void Rollback()
        {
            if (activeTransaction == null)
            {
                return;
            }

            if (this.transactions.Count == 1 && this.transactions.First() == activeTransaction.TransactionId)
            {
                try
                {
                    //Rolls back the underlying store transaction
                    activeTransaction.Rollback();
                }
                finally
                {
                    this.DisposeTransaction();
                }
            }
            else
            {
                this.transactions.Pop();
            }
        }
        //The SaveChanges() Method Implement DbContext Class SaveChanges method 
        //So whenever we do a transaction we need to call this Save() method 
        //so that it will make the changes in the database permanently
        public void SaveChanges(bool ignoreSoftDeleteFilter = false)
        {
            if (!ignoreSoftDeleteFilter)
            {
                CancelDeletionForSoftDelete();
            }
            Context.SaveChanges();
        }
        //Disposing of the Context Object
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    Context.Dispose();
            _disposed = true;
        }
        //The Dispose Method will clean up this transaction object and ensures Entity Framework
        //is no longer using that transaction.
        protected void DisposeTransaction()
        {
            activeTransaction?.Dispose();
            activeTransaction = null;
            this.transactions.Clear();
        }

        public async Task<int> SaveChangesAsync(bool ignoreSoftDeleteFilter = false, CancellationToken cancellationToken = default)
        {
            if (!ignoreSoftDeleteFilter)
            {
                await CancelDeletionForSoftDeleteAsync();
            }
            return await this.Context.SaveChangesAsync(cancellationToken);
        }

        private void CancelDeletionForSoftDelete()
        {
            foreach (var e in this.Context.ChangeTracker.Entries())
            {
                if (e.State == EntityState.Deleted && e.Entity is ISoftDelete delete)
                {
                    e.Reload();
                    e.State = EntityState.Modified;
                    delete.IsDeleted = true;
                }
            }
        }

        private async Task CancelDeletionForSoftDeleteAsync()
        {
            foreach (var e in this.Context.ChangeTracker.Entries())
            {
                if (e.State == EntityState.Deleted && e.Entity is ISoftDelete delete)
                {
                    await e.ReloadAsync();
                    e.State = EntityState.Modified;
                    delete.IsDeleted = true;
                }
            }
        }
    }
}
