using OpenAI;
using OpenAI.Embeddings;
using System.Threading.Tasks;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
}

public class OpenAIEmbeddingService : IEmbeddingService
{
    private readonly OpenAIClient _client;

    public OpenAIEmbeddingService(OpenAIClient client)
    {
        _client = client;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var embeddingClient = _client.GetEmbeddingClient("text-embedding-ada-002");
        var response = await embeddingClient.GenerateEmbeddingAsync(text);
        return response.Value.Vector.ToArray();
    }
}
