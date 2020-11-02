using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ITB.Repository.EntityFrameworkCore
{
    public static class RepositoryEntityFrameworkCoreServiceCollectionExtensions
    {
        public static RepositoryBuilder AddRepository(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var builder = new RepositoryBuilder(services);

            builder
                .AddRepository(typeof(Repository<>))
                .AddReadRepository(typeof(ReadRepository<>))
                .AddUnitOfWork<UnitOfWork>();

            return builder;
        }



        internal static bool ImplementsGenericInterface(this Type type, Type interfaceType)
            => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));

        internal static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
