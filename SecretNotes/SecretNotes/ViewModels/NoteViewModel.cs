using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using SecretNotes.Models;

namespace SecretNotes.ViewModels
{
    public class NoteViewModel
    {
        private readonly string ChildName = "notes";
        readonly FirebaseClient client = new
            FirebaseClient("https://secret-notes-9029b.firebaseio.com/");

        public async Task<List<Note>> GetAllNotes()
        {
            return (await client
                .Child(ChildName)
                .OnceAsync<Note>()).Select(item => new Note
                {
                    Text = item.Object.Text,
                    NoteID = item.Object.NoteID,
                    Date = item.Object.Date
                }).ToList();
        }

        public async Task<Note> AddNote(string text, DateTime date)
        {
            Note addedNote = new Note()
            {
                NoteID = Guid.NewGuid(),
                Text = text,
                Date = date
            };

            await client
                .Child(ChildName)
                .PostAsync(addedNote);

            return addedNote;
        }

        public async Task<Note> GetNote(Guid noteID)
        {
            var allNotes = await GetAllNotes();
            await client
                .Child(ChildName)
                .OnceAsync<Note>();
            return allNotes.FirstOrDefault(x => x.NoteID == noteID);
        }

        public async Task<Note> GetNote(string text)
        {
            var allNotes = await GetAllNotes();
            await client
                .Child(ChildName)
                .OnceAsync<Note>();
            return allNotes.FirstOrDefault(a => a.Text == text);
        }

        public async Task UpdateNote(Guid noteID, string text, DateTime date)
        {
            var toUpdateNote = (await client
                .Child(ChildName)
                .OnceAsync<Note>()).FirstOrDefault(a => a.Object.NoteID == noteID);

            await client
                .Child(ChildName)
                .Child(toUpdateNote.Key)
                .PutAsync(new Note()
                {
                    NoteID = noteID,
                    Text = text,
                    Date = date
                });
        }

        public async Task DeleteNote(Guid noteID)
        {
            var toDeleteNote = (await client
                .Child(ChildName)
                .OnceAsync<Note>()).FirstOrDefault(a => a.Object.NoteID == noteID);
            await client.Child(ChildName).Child(toDeleteNote.Key).DeleteAsync();
        }
    }
}
