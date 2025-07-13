using InsightNotes.Api.Models;
using InsightNotes.Api.Services;

public class NoteService
{
    private readonly List<Note> notes = new List<Note>();
    private readonly QdrantService qdrantService;
    private readonly IEmbeddingService embeddingService;

    public NoteService(QdrantService qdrantService, IEmbeddingService embeddingService)
    {
        this.qdrantService = qdrantService;
        this.embeddingService = embeddingService;
    }

    public IEnumerable<Note> GetNotes() => notes;

    public async Task<Note> AddNoteAsync(string title, string content)
    {
        Note note = new Note(title, content);
        notes.Add(note);

        // Generate embedding for the note (using title + content)
        var embedding = await embeddingService.GenerateEmbeddingAsync($"{title}\n{content}");

        await qdrantService.StoreNoteVectorAsync(note.Id, embedding, title, content);

        return note;
    }
}
