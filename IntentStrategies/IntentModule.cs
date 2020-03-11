using Microsoft.Extensions.DependencyInjection;

namespace DIStrategyPOC.IntentStrategies
{
    public static class IntentModule
    {
        public static IServiceCollection AddIntentServices(
            this IServiceCollection services)
        {
            return services
                .AddSingletonImplementingTypes<IIntentStrategy>()
                .AddSingleton<IStrategyHelper, StrategyHelper>()
                .AddSingleton<IIntentStrategyFactory, IntentStrategyFactory>();
        }
    }
}
