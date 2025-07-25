﻿namespace InsightNotes.Api.Models
{
    public class Note
    {
        public Note() { }

        public Note(string title, string content)
        {
            Title = title;
            Content = content;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
