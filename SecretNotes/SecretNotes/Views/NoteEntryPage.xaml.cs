using System;
using System.IO;
using Xamarin.Forms;
using SecretNotes.Models;
using SecretNotes.ViewModels;
using System.Threading.Tasks;

namespace SecretNotes.Views
{
    public partial class NoteEntryPage : ContentPage
    {
        readonly NoteViewModel noteVM;
        Note selectedNote;

        public NoteEntryPage()
        {
            noteVM = new NoteViewModel();
            InitializeComponent();
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            selectedNote = (Note)BindingContext;

            await noteVM.UpdateNote(
                selectedNote.NoteID,
                selectedNote.Text,
                selectedNote.Date);
            

            await Navigation.PopAsync();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            selectedNote = (Note)BindingContext;

            await noteVM.DeleteNote(selectedNote.NoteID);
            
            await Navigation.PopAsync();
        }
    }
}