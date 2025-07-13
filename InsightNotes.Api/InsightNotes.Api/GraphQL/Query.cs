using InsightNotes.Api.Models;

namespace InsightNotes.Api.GraphQL
{
    public class Query
    {
        private readonly NoteService noteService;

        public Query(NoteService noteService)
        {
            this.noteService = noteService;
        }

        public async Task<IEnumerable<Note>> GetNotesAsync()
        {
            return await noteService.GetNotesAsync();
        }
    }
}