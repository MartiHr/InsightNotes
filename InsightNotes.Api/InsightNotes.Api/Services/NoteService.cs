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

    public async Task<Note> UpdateNoteAsync(Guid noteId, string newTitle, string newContent)
    {
        var embedding = await embeddingService.GenerateEmbeddingAsync($"{newTitle}\n{newContent}");
        await qdrantService.StoreNoteVectorAsync(noteId, embedding, newTitle, newContent);

        return new Note
        {
            Id = noteId,
            Title = newTitle,
            Content = newContent
        };
    }

    public async Task DeleteNoteAsync(Guid noteId)
    {
        await qdrantService.DeleteNoteAsync(noteId);
    }

    public async Task<Note?> GetNoteByIdAsync(Guid noteId)
    {
        var points = await qdrantService.GetAllPointsAsync();
        var point = points.FirstOrDefault(p => Guid.TryParse(p.Id.Uuid, out var id) && id == noteId);

        if (point == null) return null;

        var title = point.Payload.TryGetValue("title", out var t) ? t.ToString() : string.Empty;
        var content = point.Payload.TryGetValue("content", out var c) ? c.ToString() : string.Empty;

        return new Note
        {
            Id = noteId,
            Title = title,
            Content = content
        };
    }

    public async Task<IEnumerable<Note>> GetNotesByIdsAsync(IEnumerable<Guid> noteIds)
    {
        var allPoints = await qdrantService.GetAllPointsAsync();

        var notes = allPoints
            .Where(p => Guid.TryParse(p.Id.Uuid, out var id) && noteIds.Contains(id))
            .Select(p =>
            {
                var title = p.Payload.TryGetValue("title", out var t) ? t.ToString() : string.Empty;
                var content = p.Payload.TryGetValue("content", out var c) ? c.ToString() : string.Empty;
                var noteIdStr = p.Payload.TryGetValue("noteId", out var idStr) ? idStr.ToString() : null;

                Guid.TryParse(noteIdStr, out Guid noteId);

                return new Note
                {
                    Id = noteId != Guid.Empty ? noteId : Guid.NewGuid(),
                    Title = title,
                    Content = content
                };
            });

        return notes;
    }

    public async Task<IEnumerable<Note>> SearchNotesAsync(string query, int limit = 5)
    {
        var queryVector = await embeddingService.GenerateEmbeddingAsync(query);
        var searchResults = await qdrantService.SearchAsync(queryVector, limit);

        var noteIds = searchResults
            .Select(r => Guid.TryParse(r.Id.Uuid, out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty);

        return await GetNotesByIdsAsync(noteIds);
    }

}
