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
            auth = new AuthViewModel();
            noteVM = new NoteViewModel();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            loadSignUp.IsRunning = false;
        }

        /// <summary>
        /// Login button handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void LoginClicked(object sender, EventArgs e)
        {
            var email = AuthViewModel.ComputeSha256Hash(EmailInput.Text);
            var pass = auth.PasswordEncrypt(PasswordInput.Text);

            auth.GetAuthToken(EmailInput.Text, PasswordInput.Text);

            if (auth.Token != "")
            {
                Application.Current.Properties.Add("email", email);
                Application.Current.Properties.Add("pass", pass);

                var code = await GetVerifyCode();
                var result = await DisplayPromptAsync("Verification Code", "Enter Below");

                if (code.ToString() == result)
                {
                    Application.Current.Properties.Add("token", auth.Token);
                    noteVM.Email = email;
                    await Navigation.PushAsync(new NotesPage());
                }
                else
                {
                    ShowError();
                    Application.Current.Properties.Remove("email");
                    Application.Current.Properties.Remove("pass");
                }
                    
            }
            else
            {
                ShowError();
            }
        }

        /// <summary>
        /// Sign Up Button Handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void SignUpClicked(object sender, EventArgs e) //Sends user to sign up page
        {
            loadSignUp.IsRunning = true;
            await Navigation.PushAsync(new SignUpPage());
            loadSignUp.IsRunning = false;
        }

        /// <summary>
        /// Verification that log in is coming from the app itself.
        /// </summary>
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

        /// <summary>
        /// Random number generator
        /// </summary>
        /// <returns></returns>
        int RandomNumber()
        {
            int min = 10000;
            int max = 99999;
            Random rand = new Random();
            return rand.Next(min, max);
        }


        async private void ShowError()
        {
            await DisplayAlert("Authentication Failed",
                $"E-mail or password are incorrect. Try again!",
                "OK");
        }
    }
}