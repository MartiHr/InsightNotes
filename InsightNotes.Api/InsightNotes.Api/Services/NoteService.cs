using InsightNotes.Api.Models;
using System.Reflection.Metadata.Ecma335;

namespace InsightNotes.Api.Services
{
    public class NoteService
    {
        private readonly List<Note> notes = new List<Note>();

        public IEnumerable<Note> GetNotes() => notes;

        public Note AddNote(string title, string content)
        {
            Note note = new Note(title, content);
            notes.Add(note);

            return note;
        }
    }
}
