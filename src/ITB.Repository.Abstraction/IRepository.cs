using System.Threading;
using System.Threading.Tasks;

namespace ITB.Repository.Abstraction
{
    public interface IRepository
    {
    }

    public interface IRepository<TEntity> : IReadRepository<TEntity>
        where TEntity : class
    {
        Task<TEntity> Add(TEntity entity, CancellationToken cancellationToken = default);

        Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken = default);

        Task<bool> Delete(TEntity entity, CancellationToken cancellationToken = default);
    }
}
