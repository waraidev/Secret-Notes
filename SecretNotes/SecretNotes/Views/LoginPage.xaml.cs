using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretNotes;
using Xamarin.Forms;

namespace SecretNotes.Views
{
    [DesignTimeVisible(true)]
    public partial class LoginPage : ContentPage
    {
        IAuth auth;

        public LoginPage()
        {
            InitializeComponent();
            auth = DependencyService.Get<IAuth>();
        }

        async void LoginClicked(object sender, EventArgs e)
        {
            string Token = await auth.LoginWithEmailPassword(EmailInput.Text, PasswordInput.Text);
            if (Token != "")
            {
                await Navigation.PushAsync(new NotesPage());
                //Application.Current.MainPage = new NotesPage();
            } 
            else
            {
                ShowError();
            }
        }

        async private void ShowError()
        {
            await DisplayAlert("Authentication Failed", "E-mail or password are incorrect. Try again!", "OK");
        }
    }
}