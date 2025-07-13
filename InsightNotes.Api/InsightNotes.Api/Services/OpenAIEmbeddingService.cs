using OpenAI;
using OpenAI.Embeddings;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
}

public class OpenAIEmbeddingService : IEmbeddingService
{
    private readonly OpenAIClient client;

    public OpenAIEmbeddingService(OpenAIClient client)
    {
        this.client = client;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var client = this.client.GetEmbeddingClient("text-embedding-3-small");

        var response = await client.GenerateEmbeddingAsync(text);

        return response.Value.ToFloats().ToArray();
    }
}