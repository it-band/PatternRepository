using System;
using ITB.Repository.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ITB.Repository.EntityFrameworkCore
{
    public static class RepositoryEntityFrameworkCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            
            services.TryAddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.TryAddTransient(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.TryAddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
