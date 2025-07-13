using InsightNotes.Api.Models;
using InsightNotes.Api.Services;

public class NoteService
{
    private readonly QdrantService qdrantService;
    private readonly IEmbeddingService embeddingService;

    public NoteService(QdrantService qdrantService, IEmbeddingService embeddingService)
    {
        this.qdrantService = qdrantService;
        this.embeddingService = embeddingService;
    }

    public async Task<IEnumerable<Note>> GetNotesAsync()
    {
        var points = await qdrantService.GetAllPointsAsync();

        return points.Select(p =>
        {
            // Extract payload fields, be careful with types and nullability
            var title = p.Payload.TryGetValue("title", out var t) ? t.ToString() : string.Empty;
            var content = p.Payload.TryGetValue("content", out var c) ? c.ToString() : string.Empty;
            var noteIdStr = p.Payload.TryGetValue("noteId", out var id) ? id.ToString() : null;

            Guid.TryParse(noteIdStr, out Guid noteId);

            return new Note
            {
                Id = noteId != Guid.Empty ? noteId : Guid.NewGuid(), // fallback if no valid GUID
                Title = title,
                Content = content
            };
        });
    }

    public async Task<Note> AddNoteAsync(string title, string content)
    {
        var note = new Note(title, content);

        // Generate embedding
        var embedding = await embeddingService.GenerateEmbeddingAsync($"{title}\n{content}");

        // Store in Qdrant
        await qdrantService.StoreNoteVectorAsync(note.Id, embedding, title, content);

        return note;
    }
}
