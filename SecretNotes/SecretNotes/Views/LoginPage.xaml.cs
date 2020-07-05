using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretNotes.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SecretNotes.Views
{
    [DesignTimeVisible(true)]
    public partial class LoginPage : ContentPage
    {
        readonly AuthViewModel auth;

        public LoginPage()
        {
            NavigationPage.SetHasBackButton(this, false);
            auth = AuthViewModel.Instance;
            InitializeComponent();
        }

        async void LoginClicked(object sender, EventArgs e)
        {
            auth.GetAuthToken(EmailInput.Text, PasswordInput.Text);
            if (auth.Token != "")
            {
                var code = await SendSms();
                var result = await DisplayPromptAsync("Verification Code", "Enter Below");

                if (code.ToString() == result)
                {
                    Application.Current.Properties.Add("token", auth.Token);
                    await Navigation.PushAsync(new NotesPage());
                }
                else
                    ShowError();
            } 
            else
            {
                ShowError();
            }
        }

        async Task<int> SendSms()
        {
            var randNum = RandomNumber();

            try
            {
                var text = $"Secret Notes Verification Code: {randNum}";
                var message = new SmsMessage(text, PhoneInput.Text);
                await Sms.ComposeAsync(message);
            }
            catch
            {
                await DisplayAlert("Error", "Verification Code Sending Failed", "OK");
            }

            return randNum;
        }

        int RandomNumber()
        {
            int min = 10000;
            int max = 99999;
            Random rand = new Random();
            return rand.Next(min, max);
        }

        async private void ShowError()
        {
            await DisplayAlert("Authentication Failed", "E-mail or password are incorrect. Try again!", "OK");
        }
    }
}