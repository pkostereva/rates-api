using RatesAPI;
using System.Collections.Generic;

namespace Messages
{
    public class RatesMessage
    {
        public List<Currency> ActualCurrencyRates { get; set; }
    }
}
