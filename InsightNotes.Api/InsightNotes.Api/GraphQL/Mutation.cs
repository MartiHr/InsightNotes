using InsightNotes.Api.Models;
using InsightNotes.Api.Services;

namespace InsightNotes.Api.GraphQL
{
    public class Mutation
    {
        public Note AddNote(string title, string content, [Service] NoteService noteService) =>
            noteService.AddNote(title, content);
    }
}
