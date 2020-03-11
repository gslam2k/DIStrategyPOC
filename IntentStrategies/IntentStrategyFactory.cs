using System;
using System.Collections.Generic;
using System.Linq;

namespace DIStrategyPOC.IntentStrategies
{

    public class IntentStrategyFactory : IIntentStrategyFactory
    {
        private readonly IDictionary<string, IIntentStrategy> _intentStrategies;

        public IntentStrategyFactory(IEnumerable<IIntentStrategy> intentStrategies)
        {
            _intentStrategies = intentStrategies.ToDictionary(x => x.Key);
        }

        public IIntentStrategy GetStrategy(UserIntent userIntent)
        {
            if (_intentStrategies.TryGetValue(
                    userIntent.Intent.ToString(),
                    out var Strategy))
            {
                return Strategy;
            }

            throw new ArgumentException(
                $"The Intent {userIntent.Intent} is not handled",
                nameof(userIntent));
        }
    }
}
