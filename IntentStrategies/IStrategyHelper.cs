using System.Threading.Tasks;

namespace DIStrategyPOC.IntentStrategies
{
    public interface IStrategyHelper
    {
         Task DisplayAsync(string intent);
    }
}