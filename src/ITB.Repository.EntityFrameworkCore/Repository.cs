using ITB.Repository.Abstraction;
using ITB.Shared.Domain;
using ITB.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ITB.Repository.EntityFrameworkCore
{
    public class Repository<TEntity> : ReadRepository<TEntity>, IRepository<TEntity>
        where TEntity : class, IEntity
    {
        public Repository(DbContext dbContext, IEnumerable<IQueryableFilter<TEntity>> filters) : base(dbContext, filters)
        {
        }

        public async Task<TEntity> Add(TEntity entity, CancellationToken cancellationToken = default)
        {
            return (await DbSet.AddAsync(entity, cancellationToken)).Entity;
        }

        public async Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken = default)
        {
            DbContext.Attach(entity);

            return await Task.FromResult(DbSet.Update(entity).Entity);
        }

        public virtual async Task<bool> Delete(TEntity entity, CancellationToken cancellationToken = default)
        {
            DbSet.Remove(entity);

            return await Task.FromResult(true);
        }
    }
}
