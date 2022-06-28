using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ITB.Repository.EntityFrameworkCore
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChanges(CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the State of all tracked Entities to Detached so that they are no longer tracked.
        /// This keeps the performance linear for commands that perform database operations recursively, such as batch operations.
        /// </summary>
        void DetachAllEntities();

        // Set timeout for command
        void SetCommandTimeout(TimeSpan timeSpan);


        /// <summary>
        /// If the entity is not attached to the DB context, it means it was obtained by the read-only context, and is attached as Unchanged.
        /// Otherwise it means it was either obtained by the write context or already attached - either way it should already by in the correct state.
        /// This should be used before creating a relationship between an entity that was potentially obtained by the read-only context
        /// and an entity that will be added/updated by the write context. Only suitable when the entity is not already tracked,
        /// as there is otherwise a chance that a different model of the same entity is already tracked and an error would be thrown.
        /// </summary>
        void AttachAsUnchangedIfMissing<TEntity>(TEntity entity) where TEntity : class;

        DbContext Context();

        Task<IDbContextTransaction> BeginTransaction();

        Task CommitTransaction();

        Task RollbackTransaction();
    }
}
