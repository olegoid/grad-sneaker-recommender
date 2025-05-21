using System.Net.Http.Headers;
using System.Text.Json;

public class MLInferenceService
{
    private readonly HttpClient _httpClient;

    public MLInferenceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResponsePayload> ProcessRequestAsync(RequestPayload request)
    {
        // Download the image to memory
        using var imageStream = await new HttpClient().GetStreamAsync(request.ImageUrl);
        using var imageContent = new StreamContent(imageStream);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        // Prepare the multipart/form-data request
        using var form = new MultipartFormDataContent
        {
            { imageContent, "file", "image.jpg" }
        };

        // Send to inference service (assumed to be running on localhost:8080)
        var response = await _httpClient.PostAsync("http://localhost:8080/predict", form);
        response.EnsureSuccessStatusCode();

        var resultJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<InferenceResult>(resultJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return new ResponsePayload
        {
            ChatId = request.ChatId,
            Recommendation = result.Recommendation,
            ImageUrl = result.ImageUrl
        };
    }
}

public class InferenceResult
{
    public string Recommendation { get; set; }
    public string ImageUrl { get; set; }
}
