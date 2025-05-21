using Azure.Messaging.ServiceBus;

public class ServiceBusConsumer : BackgroundService
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly IConfiguration _config;
    private readonly MLInferenceService _inferenceService;

    public ServiceBusConsumer(ServiceBusClient client, ServiceBusSender sender, IConfiguration config, MLInferenceService inferenceService)
    {
        _client = client;
        _sender = sender;
        _config = config;
        _inferenceService = inferenceService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var processor = _client.CreateProcessor(_config["Azure:RequestQueue"], new ServiceBusProcessorOptions());

        processor.ProcessMessageAsync += async args =>
        {
            var json = args.Message.Body.ToString();
            var request = JsonSerializer.Deserialize<RequestPayload>(json);

            var response = await _inferenceService.ProcessRequestAsync(request!);

            var responseMessage = new ServiceBusMessage(JsonSerializer.Serialize(response))
            {
                CorrelationId = request.ChatId.ToString()
            };

            await _sender.SendMessageAsync(responseMessage, stoppingToken);
            await args.CompleteMessageAsync(args.Message, stoppingToken);
        };

        processor.ProcessErrorAsync += args =>
        {
            Console.WriteLine($"Error: {args.Exception.Message}");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(stoppingToken);
    }
}
