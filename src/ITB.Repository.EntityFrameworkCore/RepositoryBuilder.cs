using System;
using System.Linq;
using System.Reflection;
using ITB.Repository.Abstraction;
using ITB.Shared.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ITB.Repository.EntityFrameworkCore
{
    public sealed class RepositoryBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> services are attached to.
        /// </summary>
        /// <value>
        /// The <see cref="IServiceCollection"/> services are attached to.
        /// </value>
        public  IServiceCollection Services { get; private set; }

        public RepositoryBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public RepositoryBuilder AddRepository(Type repositoryType)
        {
            if (!repositoryType.ImplementsGenericInterface(typeof(IRepository<>)))
            {
                throw new ArgumentException($"Type {repositoryType.Name} doesn't implement IRepository<> interface");
            }

            Services.TryAddTransient(typeof(IRepository<>), repositoryType);

            return this;
        }

        public RepositoryBuilder AddReadRepository(Type readRepositoryType)
        {
            if (!readRepositoryType.ImplementsGenericInterface(typeof(IReadRepository<>)))
            {
                throw new ArgumentException($"Type {readRepositoryType.Name} doesn't implement IReadRepository<> interface");
            }

            Services.TryAddTransient(typeof(IReadRepository<>), readRepositoryType);

            return this;
        }

        public RepositoryBuilder AddUnitOfWork<TUnitOfWork>()
        {
            return AddUnitOfWork(typeof(TUnitOfWork));
        }

        public RepositoryBuilder AddUnitOfWork(Type unitOfWorkType)
        {
            if (!typeof(IUnitOfWork).IsAssignableFrom(unitOfWorkType))
            {
                throw new ArgumentException($"Type {unitOfWorkType.Name} doesn't implement IUnitOfWork interface");
            }

            Services.TryAddTransient(typeof(IUnitOfWork), unitOfWorkType);

            return this;
        }

        public RepositoryBuilder AddQueryableFilters(params Assembly[] assemblies)
        {
            if (assemblies != null && assemblies.Length > 0)
            {
                var allTypes = assemblies
                    .Where(a => !a.IsDynamic)
                    .Distinct()
                    .SelectMany(a => a.DefinedTypes)
                    .ToArray();

                foreach (var type in allTypes
                    .Where(t => t.IsClass
                                && !t.IsAbstract
                                && t.AsType().ImplementsGenericInterface(typeof(IQueryableFilter<>))))
                {
                    var @interface = type.ImplementedInterfaces.First(i => i.IsGenericType(typeof(IQueryableFilter<>)));
                    Services.AddTransient(@interface, type.AsType());
                }
            }

            return this;
        }
    }
}
