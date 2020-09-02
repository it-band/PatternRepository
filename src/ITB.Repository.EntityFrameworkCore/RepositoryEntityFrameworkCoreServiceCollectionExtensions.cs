using System;
using System.Linq;
using System.Reflection;
using ITB.Repository.Abstraction;
using ITB.Shared.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ITB.Repository.EntityFrameworkCore
{
    public static class RepositoryEntityFrameworkCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddRepository(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            
            services.TryAddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.TryAddTransient(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.TryAddTransient<IUnitOfWork, UnitOfWork>();

            if(assemblies != null && assemblies.Length > 0)
            {
                var allTypes = assemblies
                    .Where(a => !a.IsDynamic)
                    .Distinct()
                    .SelectMany(a => a.DefinedTypes)
                    .ToArray();

                var openTypes = new[]
                    {
                    typeof(IQueryableFilter<>)
                };

                foreach (var type in openTypes.SelectMany(openType => allTypes
                    .Where(t => t.IsClass
                        && !t.IsAbstract
                        && t.AsType().ImplementsGenericInterface(openType))))
                {
                    // use try add to avoid double-registration
                    services.TryAddTransient(type.AsType());
                }
            }

            return services;
        }

        private static bool ImplementsGenericInterface(this Type type, Type interfaceType)
      => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));

        private static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
