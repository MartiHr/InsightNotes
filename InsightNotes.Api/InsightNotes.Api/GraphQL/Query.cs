using InsightNotes.Api.Models;
using InsightNotes.Api.Services;

namespace InsightNotes.Api.GraphQL
{
    public class Query
    {
        private readonly NoteService noteService;

        public Query(NoteService noteService)
        {
            this.noteService = noteService;
        }

        public async Task<IEnumerable<Note>> GetNotesAsync() =>
            await noteService.GetNotesAsync();

        public async Task<Note?> GetNoteByIdAsync(Guid noteId) =>
            await noteService.GetNoteByIdAsync(noteId);

        public async Task<IEnumerable<Note>> GetNotesByIdsAsync(IEnumerable<Guid> noteIds) =>
            await noteService.GetNotesByIdsAsync(noteIds);

        public async Task<IEnumerable<Note>> SearchNotesAsync(string query, int limit = 5) =>
            await noteService.SearchNotesAsync(query, limit);
    }
}
