using Microsoft.Extensions.Configuration;

class Program
{
    static void Main()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var bot = new SneakerRecommendationBot(
            telegramToken: configuration["TelegramToken"],
            serviceBusConnectionString: configuration["Azure:ServiceBusConnectionString"],
            requestQueue: configuration["Azure:RequestQueue"],
            responseQueue: configuration["Azure:ResponseQueue"],
            blobStorageConnection: configuration["Azure:BlobStorageConnectionString"],
            blobContainerName: configuration["Azure:BlobContainerName"]
        );

        bot.Start();
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}
