using System;

namespace SecretNotes.Models
{
    public class Note
    {
        public Guid NoteID { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; }
    }
}