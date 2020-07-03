using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using SecretNotes.Models;
using SecretNotes.ViewModels;
using System.Threading.Tasks;

namespace SecretNotes.Views
{
    public partial class NotesPage : ContentPage
    {
        readonly NoteViewModel noteVM;
        IAuth auth;

        public NotesPage()
        {
            noteVM = new NoteViewModel();
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            loading.IsRunning = true;
            await FetchNotes();
            loading.IsRunning = false;
        }

        async void OnNoteAddedClicked(object sender, EventArgs e)
        {
            var note = await noteVM.AddNote("", DateTime.Now);

            await Navigation.PushAsync(new NoteEntryPage
            {
                BindingContext = note
            });
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new NoteEntryPage
                {
                    BindingContext = e.SelectedItem as Note
                });
            }
        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            await DisplayAlert("Delete Context Action", item.CommandParameter + " delete context action", "OK");

            var note = item.BindingContext as Note;

            loading.IsRunning = true;

            await noteVM.DeleteNote(note.NoteID);
            await FetchNotes();

            loading.IsRunning = false;
        }

        private async Task FetchNotes()
        {
            var allNotes = await noteVM.GetAllNotes();

            listView.ItemsSource = allNotes
                .OrderBy(d => d.Date)
                .ToList();
        }

        private async void OnSignoutClicked(object sender, EventArgs e)
        {
            var signedOut = auth.SignOut();
            if (signedOut)
                await Navigation.PushAsync(new LoginPage());
        }
    }
}