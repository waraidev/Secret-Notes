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
        private static NoteViewModel _instance;

        readonly FirebaseClient client = new
            FirebaseClient("https://secret-notes-9029b.firebaseio.com/");

        public static NoteViewModel Instance
        {
            get => _instance ?? new NoteViewModel();
        }

        public string Email { get; set; }

        public async Task<List<Note>> GetAllNotes()
        {
            return (await client
                .Child(Email)
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
                .Child(Email)
                .PostAsync(addedNote);

            return addedNote;
        }

        public async Task<Note> GetNote(Guid noteID)
        {
            var allNotes = await GetAllNotes();
            await client
                .Child(Email)
                .OnceAsync<Note>();
            return allNotes.FirstOrDefault(x => x.NoteID == noteID);
        }

        public async Task<Note> GetNote(string text)
        {
            var allNotes = await GetAllNotes();
            await client
                .Child(Email)
                .OnceAsync<Note>();
            return allNotes.FirstOrDefault(a => a.Text == text);
        }

        public async Task UpdateNote(Guid noteID, string text, DateTime date)
        {
            var toUpdateNote = (await client
                .Child(Email)
                .OnceAsync<Note>()).FirstOrDefault(a => a.Object.NoteID == noteID);

            await client
                .Child(Email)
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
                .Child(Email)
                .OnceAsync<Note>()).FirstOrDefault(a => a.Object.NoteID == noteID);
            await client.Child(Email).Child(toDeleteNote.Key).DeleteAsync();
        }
    }
}
