using InsightNotes.Api.Models;
using InsightNotes.Api.Services;

namespace InsightNotes.Api.GraphQL
{
    public class Query
    {
        public IEnumerable<Note> GetNotes([Service] NoteService noteService) =>
            noteService.GetNotes();
    }
}
