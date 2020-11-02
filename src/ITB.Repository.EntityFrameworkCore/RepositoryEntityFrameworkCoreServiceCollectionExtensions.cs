using System;
using System.Linq;
using System.Reflection;
using ITB.Repository.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ITB.Repository.EntityFrameworkCore
{
    public static class RepositoryEntityFrameworkCoreServiceCollectionExtensions
    {
        public static RepositoryBuilder AddRepositories(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.TryAddTransient(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.TryAddTransient<IUnitOfWork, UnitOfWork>();

            return new RepositoryBuilder(services);
        }

        internal static bool ImplementsGenericInterface(this Type type, Type interfaceType)
            => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));

        internal static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
