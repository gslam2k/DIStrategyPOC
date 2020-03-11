using System;
using System.Threading.Tasks;
using DIStrategyPOC.IntentStrategies;
using Microsoft.Extensions.DependencyInjection;

namespace DIStrategyPOC
{
    class Program
    {
        public static async Task Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddIntentServices()
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<IIntentStrategyFactory>();

            foreach (var intent in new []
                {
                    new UserIntent { Intent = Intent.Delay },
                    new UserIntent { Intent = Intent.Available },
                    new UserIntent { Intent = Intent.Average },
                    new UserIntent { Intent = Intent.Other },
                })
            {
                try
                {
                    await factory.GetStrategy(intent).ProcessIntentAsync(intent);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
