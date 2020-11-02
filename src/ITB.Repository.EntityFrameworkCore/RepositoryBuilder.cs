using System;
using System.Linq;
using System.Reflection;
using ITB.Repository.Abstraction;
using ITB.Shared.Domain;
using Microsoft.Extensions.DependencyInjection;

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
        public  IServiceCollection Services { get; }

        public RepositoryBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public RepositoryBuilder SetRepository(Type repositoryType)
        {
            if (!repositoryType.ImplementsGenericInterface(typeof(IRepository<>)))
            {
                throw new ArgumentException($"Type {repositoryType.Name} doesn't implement IRepository<> interface");
            }

            Services.AddTransient(typeof(IRepository<>), repositoryType);

            return this;
        }

        public RepositoryBuilder SetReadRepository(Type readRepositoryType)
        {
            if (!readRepositoryType.ImplementsGenericInterface(typeof(IReadRepository<>)))
            {
                throw new ArgumentException($"Type {readRepositoryType.Name} doesn't implement IReadRepository<> interface");
            }

            Services.AddTransient(typeof(IReadRepository<>), readRepositoryType);

            return this;
        }

        public RepositoryBuilder SetUnitOfWork<TUnitOfWork>()
        {
            return SetUnitOfWork(typeof(TUnitOfWork));
        }

        public RepositoryBuilder SetUnitOfWork(Type unitOfWorkType)
        {
            if (!typeof(IUnitOfWork).IsAssignableFrom(unitOfWorkType))
            {
                throw new ArgumentException($"Type {unitOfWorkType.Name} doesn't implement IUnitOfWork interface");
            }

            Services.AddTransient(typeof(IUnitOfWork), unitOfWorkType);

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
