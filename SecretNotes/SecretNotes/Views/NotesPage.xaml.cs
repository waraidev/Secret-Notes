using System;
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
        readonly AuthViewModel auth;

        public NotesPage()
        {
            auth = AuthViewModel.Instance;
            NavigationPage.SetHasBackButton(this, false);
            noteVM = NoteViewModel.Instance;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            loading.IsRunning = true;
            await FetchNotes();
            loading.IsRunning = false;
        }

        /// <summary>
        /// Add button handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnNoteAddedClicked(object sender, EventArgs e)
        {
            var note = await noteVM.AddNote("", DateTime.Now);

            await Navigation.PushAsync(new NoteEntryPage
            {
                BindingContext = note
            });
        }

        /// <summary>
        /// Item select handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Delete button handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnDeleteClicked(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            var delete = await DisplayAlert("Delete Item",
                "Are you sure you want to delete this note?", "Yes", "No");

            var note = item.BindingContext as Note;

            if (delete)
            {
                loading.IsRunning = true;

                await noteVM.DeleteNote(note.NoteID);
                await FetchNotes();

                loading.IsRunning = false;
            }
        }

        /// <summary>
        /// Fetches notes for the list view.
        /// </summary>
        /// <returns></returns>
        private async Task FetchNotes()
        {
            try
            {
                var allNotes = await noteVM.GetAllNotes();

                listView.ItemsSource = allNotes
                .OrderBy(d => d.Date)
                .ToList();
            }
            catch
            {
                var signout = !await DisplayAlert("Bad Connection",
                    "Check your connectivity and try again later!",
                    "Reconnect", "Signout?");

                if (signout)
                    await SignOut();
                else
                    OnAppearing();
            }
        }

        /// <summary>
        /// Change Password button handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnChangePassClicked(object sender, EventArgs e)
        {
            var change = await DisplayAlert("Password Change",
                "Did you mean to change?", "Yes", "No");
            if (change)
            {
                string newPass = await DisplayPromptAsync("Change Password", "Enter");
                if (newPass != "")
                {
                    if(auth.PasswordChecker(newPass))
                    {
                        auth.Password = (string)Application.Current.Properties["pass"];
                        if (auth.IteratedPasswordsCheck(newPass))
                        {
                            try
                            {
                                auth.UpdatePassword(newPass);
                                await SignOut();
                            }
                            catch (Exception ex)
                            {
                                await DisplayAlert("Update Password Failed",
                                    $"Error: {ex.Message}", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Update Password Failed",
                                "Password was too similar to last password", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Update Password Failed",
                            "Password must contain a lowercase " +
                            "letter, an upper case letter, " +
                            "a special character, and number!", "OK");
                    }
                }
            }      
        }

        /// <summary>
        /// Signout handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnSignoutClicked(object sender, EventArgs e)
        {
            await SignOut();  
        }

        /// <summary>
        /// Signs out of current account and navigates to login page.
        /// </summary>
        private async Task SignOut()
        {
            var signedOut = auth.SignOut();
            if (signedOut)
            {
                Application.Current.Properties.Remove("token");
                Application.Current.Properties.Remove("email");
                Application.Current.Properties.Remove("pass");

                await Navigation.PushAsync(new LoginPage());
            }
        }
    }
}