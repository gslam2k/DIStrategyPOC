namespace DIStrategyPOC.IntentStrategies
{
    public interface IIntentStrategyFactory
    {
        IIntentStrategy GetStrategy(UserIntent userIntent);
    }
}