using ITB.Repository.Abstraction;
using ITB.Shared.Domain;
using ITB.Shared.Domain.Entities;
using ITB.Specification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ITB.Repository.EntityFrameworkCore
{
    public class ReadRepository<TEntity> : IReadRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected readonly DbContext DbContext;
        protected readonly DbSet<TEntity> DbSet;
        protected readonly IEnumerable<IQueryableFilter<TEntity>> Filters;

        public ReadRepository(DbContext dbContext, IEnumerable<IQueryableFilter<TEntity>> filters)
        {
            DbContext = dbContext;
            Filters = filters;
            DbSet = dbContext.Set<TEntity>();
        }

        protected IQueryable<TEntity> BaseQuery
        {
            get
            {
                var query = DbSet.AsQueryable();

                if (Filters == null)
                {
                    return query;
                }

                foreach (var filter in Filters)
                {
                    query = filter.Apply(query);
                }

                return query;
            }
        }
        public IQueryable<TEntity> Query(Specification<TEntity> specification)
        {
            var query = BaseQuery;

            // fetch a Queryable that includes all expression-based includes
            query = specification.Includes
                .Aggregate(query,
                    (current, include) => current.Include(include));

            // modify the IQueryable to include any string-based include statements
            query = specification.IncludeStrings
                .Aggregate(query,
                    (current, include) => current.Include(include));

            return query.Where(specification);
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> expression)
        {
            return BaseQuery.Where(expression);
        }

        public IQueryable<TEntity> Query()
        {
            return BaseQuery;
        }

        public async Task<TEntity> FirstOrDefault(Specification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            var query = BaseQuery;

            // fetch a Queryable that includes all expression-based includes
            query = specification.Includes
                .Aggregate(query,
                    (current, include) => current.Include(include));

            // modify the IQueryable to include any string-based include statements
            query = specification.IncludeStrings
                .Aggregate(query,
                    (current, include) => current.Include(include));

            return await query.Where(specification).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await BaseQuery.FirstOrDefaultAsync(expression, cancellationToken);
        }

        public async Task<int> Count(CancellationToken cancellationToken = default)
        {
            return await BaseQuery.CountAsync(cancellationToken);
        }

        public async Task<int> Count(Specification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            return await Count(specification.Predicate, cancellationToken);
        }

        public async Task<int> Count(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await BaseQuery.CountAsync(expression, cancellationToken);
        }

        public async Task<bool> Exists(Specification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            var count = await Count(specification, cancellationToken);

            return count > 0;
        }

        public async Task<bool> Exists(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            var count = await Count(expression, cancellationToken);

            return count > 0;
        }
    }
}
