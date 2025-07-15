using InsightNotes.Api.Models;
using InsightNotes.Api.Services;

namespace InsightNotes.Api.GraphQL
{
    public class Mutation
    {
        public async Task<Note> AddNoteAsync(string title, string content, [Service] NoteService noteService) =>
            await noteService.AddNoteAsync(title, content);

        public async Task<Note> UpdateNoteAsync(Guid noteId, string newTitle, string newContent, [Service] NoteService noteService) =>
            await noteService.UpdateNoteAsync(noteId, newTitle, newContent);

        public async Task<bool> DeleteNoteAsync(Guid noteId, [Service] NoteService noteService)
        {
            await noteService.DeleteNoteAsync(noteId);
            return true;
        }
    }
}
