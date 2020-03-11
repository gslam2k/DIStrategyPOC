using System;
using System.Threading.Tasks;

namespace DIStrategyPOC.IntentStrategies
{
    public class StrategyHelper: IStrategyHelper
    {
         public async Task DisplayAsync(string intent)
         {
             await Task.Delay(100);
             Console.WriteLine($"Display {intent}");
         }
    }
}