using System.Diagnostics;
using System.Text.Json;

public class SneakerSearchService
{
    public async Task<(string RecommendationText, string PreviewImageUrl)> FindSimilarAsync(string imagePath, string? textQuery)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "python3",
            Arguments = $"model_runner.py \"{imagePath}\" \"{textQuery}\"",
            RedirectStandardOutput = true
        };

        using var proc = Process.Start(psi);
        var output = await proc!.StandardOutput.ReadToEndAsync();
        proc.WaitForExit();

        var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(output);
        return (parsed["recommendation"], parsed["image_url"]);
    }
}
