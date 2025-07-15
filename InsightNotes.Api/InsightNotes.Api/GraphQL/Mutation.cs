using InsightNotes.Api.Models;
using InsightNotes.Api.Services;

namespace InsightNotes.Api.GraphQL
{
    public class Mutation
    {
        private readonly NoteService noteService;

        public Mutation(NoteService noteService)
        {
            this.noteService = noteService;
        }

        public async Task<Note> AddNoteAsync(string title, string content) =>
            await noteService.AddNoteAsync(title, content);

        public async Task<Note> UpdateNoteAsync(Guid noteId, string newTitle, string newContent) =>
            await noteService.UpdateNoteAsync(noteId, newTitle, newContent);

        public async Task<bool> DeleteNoteAsync(Guid noteId)
        {
            await noteService.DeleteNoteAsync(noteId);
            return true; // or some status if you want to handle failure
        }
    }
}
