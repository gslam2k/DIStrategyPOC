using System.Threading.Tasks;

namespace DIStrategyPOC.IntentStrategies
{
    public class IntentStrategyDelay : BaseIntentStrategy
    {
        public IntentStrategyDelay(IStrategyHelper StrategyHelper) : base(StrategyHelper) { }
        public override string Key => "Delay";
        public override async Task ProcessIntentAsync(UserIntent userIntent) => await StrategyHelper.DisplayAsync($"{userIntent.Intent} in {nameof(IntentStrategyDelay)}");
    }
}
