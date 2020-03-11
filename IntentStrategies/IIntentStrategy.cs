using System.Threading.Tasks;

namespace DIStrategyPOC.IntentStrategies
{
public interface IIntentStrategy
{
    string Key { get; }

    Task ProcessIntentAsync(UserIntent userIntent);
}
}
