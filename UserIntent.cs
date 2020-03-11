using System;

namespace DIStrategyPOC
{
    public class UserIntent
    {
        public string Wharf { get; set;}
        public string Berth { get; set; }
        public DateTime AsOfTimestamp {get; set; }

        public Intent Intent {get; set;}
    }
}