using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class HuggingFaceEmbeddingService : IEmbeddingService
{
    private readonly HttpClient httpClient;
    private readonly string apiUrl = "https://api-inference.huggingface.co/pipeline/feature-extraction/sentence-transformers/all-MiniLM-L6-v2";
    private readonly string apiKey;

    public HuggingFaceEmbeddingService(HttpClient httpClient, IConfiguration config)
    {
        this.httpClient = httpClient;
        apiKey = config["HuggingFace:ApiKey"]
                  ?? throw new InvalidOperationException("HuggingFace API key is missing from configuration.");
    }

    public async Task<float[]> GenerateEmbeddingAsync(string input)
    {
        var payload = new { inputs = input };
        var json = JsonSerializer.Serialize(payload);

        using var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"HuggingFace API Error: {response.StatusCode} — {error}");
        }

        var resultJson = await response.Content.ReadAsStringAsync();
        var embedding = JsonSerializer.Deserialize<float[][]>(resultJson);
        return embedding?[0] ?? throw new Exception("Empty embedding received.");
    }
}
