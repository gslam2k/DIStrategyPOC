using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DIStrategyPOC
{
    public static class RegistrationExtensions {
        private static readonly Lazy<IList<Type>> ExecutingTypes = new Lazy<IList<Type>> (
            () => Assembly.GetExecutingAssembly ().ExportedTypes
            .Where (t => !t.IsAbstract && !t.IsInterface)
            .ToList ());

        public static IServiceCollection AddSingletonImplementingTypes<IType> (
            this IServiceCollection services) {
            return services.AddImplementingTypes<IType> (
                (serv, ityp, typ) => serv.AddSingleton (ityp, typ));
        }

        public static IServiceCollection AddScopedImplementingTypes<IType> (
            this IServiceCollection services) {
            return services.AddImplementingTypes<IType> (
                (serv, ityp, typ) => serv.AddScoped (ityp, typ));
        }

        public static IServiceCollection AddTransientImplementingTypes<IType> (
            this IServiceCollection services) {
            return services.AddImplementingTypes<IType> (
                (serv, ityp, typ) => serv.AddTransient (ityp, typ));
        }

        private static IServiceCollection AddImplementingTypes<IType> (
            this IServiceCollection services, Action<IServiceCollection, Type, Type> addScopeAction) {
            var interfaceType = typeof (IType);

            foreach (var type in ExecutingTypes.Value.
                Where (interfaceType.IsAssignableFrom)) {
                addScopeAction (services, interfaceType, type);
            }

            return services;
        }
    }
}