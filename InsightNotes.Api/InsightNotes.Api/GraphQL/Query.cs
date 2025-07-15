using InsightNotes.Api.Models;
using InsightNotes.Api.Services;

namespace InsightNotes.Api.GraphQL
{
    public class Query
    {
        public async Task<IEnumerable<Note>> GetNotesAsync([Service] NoteService noteService) =>
            await noteService.GetNotesAsync();

        public async Task<Note?> GetNoteByIdAsync(Guid noteId, [Service] NoteService noteService) =>
            await noteService.GetNoteByIdAsync(noteId);

        public async Task<IEnumerable<Note>> GetNotesByIdsAsync(IEnumerable<Guid> noteIds, [Service] NoteService noteService) =>
            await noteService.GetNotesByIdsAsync(noteIds);

        public async Task<IEnumerable<Note>> SearchNotesAsync(string query, int limit, [Service] NoteService noteService) =>
            await noteService.SearchNotesAsync(query, limit);
    }
}
