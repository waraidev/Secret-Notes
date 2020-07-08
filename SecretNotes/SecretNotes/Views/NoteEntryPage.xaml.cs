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
            NavigationPage.SetBackButtonTitle(this, "Notes");
            noteVM = NoteViewModel.Instance;
            InitializeComponent();
        }

        /// <summary>
        /// Handles when the save button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            selectedNote = (Note)BindingContext;

            await noteVM.UpdateNote(
                selectedNote.NoteID,
                selectedNote.Text,
                selectedNote.Date,
                noteVM.Email);
            

            await Navigation.PopAsync();
        }

        /// <summary>
        /// Handles when the Delete button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            selectedNote = (Note)BindingContext;

            await noteVM.DeleteNote(selectedNote.NoteID);
            
            await Navigation.PopAsync();
        }
    }
}