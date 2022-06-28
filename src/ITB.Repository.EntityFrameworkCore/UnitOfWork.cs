using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ITB.Repository.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext _dbContext;
        private bool _disposed;

        public UnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void DetachAllEntities()
        {
            _dbContext.ChangeTracker.Clear();
        }

        public void SetCommandTimeout(TimeSpan timeSpan)
        {            
            _dbContext.Database.SetCommandTimeout(timeSpan);
        }

        public DbContext Context()
        {
            return _dbContext;
        }

        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            await _dbContext.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransaction()
        {
            await _dbContext.Database.RollbackTransactionAsync();
        }

        /// <inheritdoc />
        public void AttachAsUnchangedIfMissing<TEntity>(TEntity entity) where TEntity : class
        {
            // if entity is not already attached to this context
            if (entity != null && _dbContext.ChangeTracker.Entries().All(x => x.Entity != entity))
            {
                // attach if not already attached, and set state to Unchanged
                _dbContext.Entry(entity).State = EntityState.Unchanged;
            }
        }

        public void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_dbContext != null)
                {
                    _dbContext.Dispose();
                    _dbContext = null;
                }
            }

            _disposed = true;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
