using Microsoft.Extensions.DependencyInjection;

namespace DIStrategyPOC.IntentStrategies
{
    public static class IntentModule
    {
        public static IServiceCollection AddIntentServices(
            this IServiceCollection services)
        {
            return services
                .AddTransientImplementingTypes<IIntentStrategy>()
                .AddTransient<IStrategyHelper, StrategyHelper>()
                .AddTransient<IIntentStrategyFactory, IntentStrategyFactory>();
        }
    }
}
