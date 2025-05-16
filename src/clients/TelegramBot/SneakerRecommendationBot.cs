using System.Text.Json;
using System.Text.Json.Serialization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;

public class SneakerRecommendationBot
{
    private readonly ITelegramBotClient _botClient;
    private readonly ServiceBusClient _sbClient;
    private readonly ServiceBusSender _requestSender;
    private readonly ServiceBusReceiver _responseReceiver;
    private readonly BlobContainerClient _blobContainer;

    public SneakerRecommendationBot(
        string telegramToken,
        string serviceBusConnectionString,
        string requestQueue,
        string responseQueue,
        string blobStorageConnection,
        string blobContainerName)
    {
        _botClient = new TelegramBotClient(telegramToken);

        _sbClient = new ServiceBusClient(serviceBusConnectionString);
        _requestSender = _sbClient.CreateSender(requestQueue);
        _responseReceiver = _sbClient.CreateReceiver(responseQueue);

        _blobContainer = new BlobContainerClient(blobStorageConnection, blobContainerName);
        _blobContainer.CreateIfNotExists();
    }

    public void Start()
    {
        var cts = new CancellationTokenSource();
        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
            cancellationToken: cts.Token);

        Console.WriteLine("Telegram bot started...");
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Type != UpdateType.Message || update.Message == null)
            return;

        var msg = update.Message;
        var chatId = msg.Chat.Id;
        string userText = msg.Text ?? "";
        string imageUrl = null;

        if (msg.Photo != null && msg.Photo.Any())
        {
            var photo = msg.Photo.Last();
            var file = await bot.GetFile(photo.FileId, ct);
            using var stream = new MemoryStream();
            await bot.DownloadFile(file.FilePath, stream, ct);
            stream.Position = 0;

            var blobName = $"{chatId}_{photo.FileUniqueId}.jpg";
            var blobClient = _blobContainer.GetBlobClient(blobName);
            await blobClient.UploadAsync(stream, overwrite: true);
            imageUrl = blobClient.Uri.ToString();
        }

        await bot.SendMessage(chatId, "Processing your request...", cancellationToken: ct);

        var requestPayload = new
        {
            ChatId = chatId,
            Timestamp = DateTime.UtcNow,
            Text = userText,
            ImageUrl = imageUrl
        };

        var sbMessage = new ServiceBusMessage(JsonSerializer.Serialize(requestPayload))
        {
            CorrelationId = chatId.ToString()
        };

        await _requestSender.SendMessageAsync(sbMessage, ct);

        var response = await WaitForResultAsync(chatId.ToString(), timeoutSeconds: 30, ct);

        if (response == null)
        {
            await bot.SendMessage(chatId, "Sorry, no recommendations were received in time.", cancellationToken: ct);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(response.Recommendation))
            {
                await bot.SendMessage(chatId, response.Recommendation, cancellationToken: ct);
            }

            if (!string.IsNullOrWhiteSpace(response.ImageUrl))
            {
                await bot.SendPhoto(
                    chatId: chatId,
                    photo: InputFile.FromUri(response.ImageUrl),
                    caption: "Suggested match:",
                    cancellationToken: ct);
            }
        }
    }

    private async Task<BotResponse?> WaitForResultAsync(string chatId, int timeoutSeconds, CancellationToken ct)
    {
        var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);

        while (DateTime.UtcNow < deadline)
        {
            var messages = await _responseReceiver.ReceiveMessagesAsync(maxMessages: 5, maxWaitTime: TimeSpan.FromSeconds(2), ct);

            foreach (var msg in messages)
            {
                var json = msg.Body.ToString();
                var response = JsonSerializer.Deserialize<BotResponse>(json);

                if (response != null && response.ChatId == chatId)
                {
                    return response;
                }
            }
        }

        return null;
    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
    {
        Console.WriteLine($"Bot error: {exception.Message}");
        return Task.CompletedTask;
    }

    private class BotResponse
    {
        [JsonPropertyName("ChatId")]
        public string ChatId { get; set; }

        [JsonPropertyName("Recommendation")]
        public string Recommendation { get; set; }

        [JsonPropertyName("ImageUrl")]
        public string ImageUrl { get; set; }
    }
}
