using MassTransit;
using Messages;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Timers;

namespace RatesAPI
{
    public class RatesPublisher
    {
        private Timer _timer;
        private IBusControl _busControl;

        public RatesPublisher(IBusControl busControl)
        {
            _busControl = busControl;
        }
        public void SetTimer()
        {
            GetRates();
            PublishRates();
            _timer = new Timer(60000);
            _timer.Elapsed += GetRates;
            _timer.Elapsed += PublishRates;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void GetRates(object source = null, ElapsedEventArgs e = null)
        {
            List<Currency> currencies = new List<Currency>() 
            {
                new Currency
                {
                    Code = "RUB",
                    Rate = (decimal)73.162
                },
                new Currency
                {
                    Code = "EUR",
                    Rate = (decimal)0.924
                },
                new Currency
                {
                    Code = "JPY",
                    Rate = (decimal)107.149
                }};

            CurrencyRates.ActualCurrencyRates = currencies;
        }

        private async void PublishRates(object source = null, ElapsedEventArgs e = null)
        {
            await _busControl.StartAsync();

            await _busControl.Publish<RatesMessage>(new
            {
                CurrencyRates.ActualCurrencyRates
            });
        }

        private List<Currency> GetRatesRequest()
        {
            List<Currency> currencies = new List<Currency>();
            var client = new RestClient("https://www.freeforexapi.com/api");
            var request = new RestRequest("live?pairs=USDEUR,USDRUB,USDJPY", Method.GET);
            var response = client.Execute(request);
            JObject jsonObj = JObject.Parse(response.Content);

            foreach (var item in Enum.GetValues(typeof(CurrencyEnum)))
            {
                decimal rate;
                if (jsonObj.SelectToken($"rates.{item}.rate") != null)
                    rate = (decimal)jsonObj.SelectToken($"rates.{item}.rate");
                else
                    rate = 1;
                Currency currency = new Currency()
                {
                    Code = item.ToString(),
                    Rate = rate
                };
                currencies.Add(currency);
            }
            return currencies;
        }
    }
}