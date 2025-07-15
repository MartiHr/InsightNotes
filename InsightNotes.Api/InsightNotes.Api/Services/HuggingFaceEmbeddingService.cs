using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class HuggingFaceEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HuggingFaceEmbeddingService> _logger;

    public HuggingFaceEmbeddingService(HttpClient httpClient, ILogger<HuggingFaceEmbeddingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string input)
    {
        try
        {
            var embeddings = await TryGetEmbedding(
                "models/sentence-transformers/all-MiniLM-L6-v2/pipeline/feature-extraction",
                new { inputs = new[] { input } });

            if (embeddings == null)
            {
                _logger.LogWarning("Primary endpoint failed, trying fallback...");

                embeddings = await TryGetEmbedding(
                    "models/sentence-transformers/all-MiniLM-L6-v2",
                    new { inputs = new[] { input } });
            }

            if (embeddings == null || embeddings.Length == 0)
            {
                throw new Exception("All API endpoints failed to generate embeddings");
            }

            return embeddings[0];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Embedding generation failed");
            throw;
        }
    }

    private async Task<float[][]?> TryGetEmbedding(string endpoint, object payload)
    {
        try
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                _logger.LogInformation("Model loading, waiting 30 seconds...");
                await Task.Delay(30000);
                response = await _httpClient.PostAsync(endpoint, content);
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Endpoint {Endpoint} failed: {StatusCode} - {Error}",
                    endpoint, response.StatusCode, error);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<float[][]>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to call endpoint {Endpoint}", endpoint);
            return null;
        }
    }
}
