public class MLInferenceService
{
    private readonly SneakerSearchService _search;

    public MLInferenceService(SneakerSearchService search)
    {
        _search = search;
    }

    public async Task<ResponsePayload> ProcessRequestAsync(RequestPayload request)
    {
        // Save the image locally or stream to Python
        var tempPath = Path.GetTempFileName();
        using var client = new HttpClient();
        await using var stream = await client.GetStreamAsync(request.ImageUrl);
        await using var file = File.Create(tempPath);
        await stream.CopyToAsync(file);

        // Run Python script for image and (optionally) text embedding
        var result = await _search.FindSimilarAsync(tempPath, request.Text);

        return new ResponsePayload
        {
            ChatId = request.ChatId,
            Recommendation = result.RecommendationText,
            ImageUrl = result.PreviewImageUrl
        };
    }
}
