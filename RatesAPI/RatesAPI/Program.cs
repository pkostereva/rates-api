using System;
using System.Threading.Tasks;
using MassTransit;


namespace RatesAPI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg => cfg.Host("localhost"));

            RatesPublisher ratesPublisher = new RatesPublisher(busControl);
            ratesPublisher.SetTimer();

            Console.ReadLine();

            await busControl.StopAsync();
        }
    }
}
