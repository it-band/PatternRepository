using System;
using System.Threading;
using System.Threading.Tasks;

namespace ITB.Repository.Abstraction
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChanges(CancellationToken cancellationToken = default);
    }
}
