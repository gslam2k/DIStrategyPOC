using System.Threading.Tasks;

namespace DIStrategyPOC.IntentStrategies
{
    public abstract class BaseIntentStrategy : IIntentStrategy
    {
        public BaseIntentStrategy(IStrategyHelper strategyHelper)
        {
            StrategyHelper = strategyHelper;
        }

        protected IStrategyHelper StrategyHelper { get; }

        public abstract string Key { get; }

        public abstract Task ProcessIntentAsync(UserIntent userIntent);
    }
}
