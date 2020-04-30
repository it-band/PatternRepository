using System;
using System.Threading;
using System.Threading.Tasks;
using ITB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;

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
