using System.Threading.Tasks;

namespace DIStrategyPOC.IntentStrategies
{
    public class IntentStrategyAverage : BaseIntentStrategy
    {
        public IntentStrategyAverage(IStrategyHelper StrategyHelper) : base(StrategyHelper) { }
        public override string Key => "Average";
        public override async Task ProcessIntentAsync(UserIntent userIntent) => await StrategyHelper.DisplayAsync($"{userIntent.Intent} in {nameof(IntentStrategyAverage)}");
    }
}
