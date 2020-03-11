using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DIStrategyPOC
{
    public static class RegistrationExtensions
    {
        private static readonly Lazy<IList<Type>> ExecutingTypes = new Lazy<IList<Type>>(
            () => Assembly.GetExecutingAssembly().ExportedTypes
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToList());

        public static IServiceCollection AddSingletonImplementingTypes<IType>(
            this IServiceCollection services)
        {
            var interfaceType = typeof(IType);

            foreach (var type in ExecutingTypes.Value.
                Where(interfaceType.IsAssignableFrom))
            {
                services.AddSingleton(interfaceType, type);
            }

            return services;
        }
    }
}
