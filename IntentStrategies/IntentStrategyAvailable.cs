using System.Threading.Tasks;

namespace DIStrategyPOC.IntentStrategies
{
    public class IntentStrategyAvailable : BaseIntentStrategy
    {
        public IntentStrategyAvailable(IStrategyHelper StrategyHelper) : base(StrategyHelper) { }

        public override string Key => "Available";

        public override async Task ProcessIntentAsync(UserIntent userIntent) => await StrategyHelper.DisplayAsync($"{userIntent.Intent} in {nameof(IntentStrategyAvailable)}");
    }
}
