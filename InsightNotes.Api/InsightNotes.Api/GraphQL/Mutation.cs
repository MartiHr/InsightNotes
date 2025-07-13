using InsightNotes.Api.Models;


namespace InsightNotes.Api.GraphQL
{
    public class Mutation
    {
        public async Task<Note> AddNoteAsync(string title, string content, [Service] NoteService noteService) =>
            await noteService.AddNoteAsync(title, content);
    }
}

