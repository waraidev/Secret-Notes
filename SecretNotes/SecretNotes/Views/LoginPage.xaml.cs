using System;
using System.ComponentModel;
using System.Threading.Tasks;
using SecretNotes.ViewModels;
using Xamarin.Forms;

namespace SecretNotes.Views
{
    [DesignTimeVisible(true)]
    public partial class LoginPage : ContentPage
    {
        readonly AuthViewModel auth;
        readonly NoteViewModel noteVM;

        public LoginPage()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            auth = AuthViewModel.Instance;
            noteVM = NoteViewModel.Instance;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            loadSignUp.IsRunning = false;
        }

        

        async void LoginClicked(object sender, EventArgs e)
        {
            auth.GetAuthToken(EmailInput.Text, PasswordInput.Text);
            if (auth.Token != "")
            {
                var code = await GetVerifyCode();
                var result = await DisplayPromptAsync("Verification Code", "Enter Below");

                if (code.ToString() == result)
                {
                    Application.Current.Properties.Add("token", auth.Token);
                    noteVM.Email = EmailInput.Text;
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

        async void SignUpClicked(object sender, EventArgs e) //Sends user to sign up page
        {
            loadSignUp.IsRunning = true;
            await Navigation.PushAsync(new SignUpPage());
            loadSignUp.IsRunning = false;
        }

        async Task<int> GetVerifyCode()
        {
            var randNum = RandomNumber();

            try
            {
                var text = $"Secret Notes Verification Code: {randNum}.";
                await DisplayAlert("Verification Code",
                    $"{text} Remember this code before clicking Dismiss!",
                    "Dismiss");
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