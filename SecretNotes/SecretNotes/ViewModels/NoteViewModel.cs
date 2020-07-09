using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using SecretNotes.Models;
using Xamarin.Forms;

namespace SecretNotes.ViewModels
{
    public class NoteViewModel
    {
        private readonly string ChildName = "notes";

        private static NoteViewModel _instance;

        private readonly AuthViewModel auth;

        private readonly FirebaseClient client;

        public string Email { get; set; }

        /// <summary>
        /// This uses the Singleton design pattern to
        /// make sure that all classes use the same instance
        /// </summary>
        public static NoteViewModel Instance
        {
            get
            {
                _instance ??= new NoteViewModel();
                return _instance;
            }
        }

        public NoteViewModel()
        {
            _instance = this;
            auth = AuthViewModel.Instance;
            client = new FirebaseClient(
                "https://secret-notes-9029b.firebaseio.com/",
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(auth.Token)
                }
            );

            if (Application.Current.Properties.ContainsKey("email"))
                Email = (string)Application.Current.Properties["email"];
        }

        /// <summary>
        /// Gets all notes for the specified user
        /// </summary>
        public async Task<List<Note>> GetAllNotes()
        {
            return (await client
                .Child(ChildName)
                .OnceAsync<Note>()).Where(item => item.Object.Email == Email)
                .Select(item => new Note
                {
                    Text = item.Object.Text,
                    NoteID = item.Object.NoteID,
                    Date = item.Object.Date,
                    Email = item.Object.Email
                }).ToList();
        }

        /// <summary>
        /// Adds a note to Firebase.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="date"></param>
        public async Task<Note> AddNote(string text, DateTime date)
        {
            Note addedNote = new Note()
            {
                NoteID = Guid.NewGuid(),
                Text = text,
                Date = date,
                Email = Email //right hand side is from NoteViewModel
            };

            await client
                .Child(ChildName)
                .PostAsync(addedNote);

            return addedNote;
        }

        /// <summary>
        /// Gets a note from Firebase.
        /// </summary>
        /// <param name="noteID"></param>
        public async Task<Note> GetNote(Guid noteID)
        {
            var allNotes = await GetAllNotes();
            await client
                .Child(ChildName)
                .OnceAsync<Note>();
            return allNotes.FirstOrDefault(x => x.NoteID == noteID);
        }

        /// <summary>
        /// Gets a note from Firebase.
        /// </summary>
        /// <param name="text"></param>
        public async Task<Note> GetNote(string text)
        {
            var allNotes = await GetAllNotes();
            await client
                .Child(ChildName)
                .OnceAsync<Note>();
            return allNotes.FirstOrDefault(a => a.Text == text);
        }

        /// <summary>
        /// Updates a note already in Firebase.
        /// </summary>
        /// <param name="noteID"></param>
        /// <param name="text"></param>
        /// <param name="date"></param>
        /// <param name="email"></param>
        public async Task UpdateNote(Guid noteID, string text,
            DateTime date, string email)
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
                    Date = date,
                    Email = email
                });
        }

        /// <summary>
        /// Deletes a note in Firebase.
        /// </summary>
        /// <param name="noteID"></param>
        public async Task DeleteNote(Guid noteID)
        {
            var toDeleteNote = (await client
                .Child(ChildName)
                .OnceAsync<Note>()).FirstOrDefault(a => a.Object.NoteID == noteID);

            await client.Child(ChildName).Child(toDeleteNote.Key).DeleteAsync();
        }
    }
}
