using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SecretNotes.ViewModels;
using Xamarin.Forms;

namespace SecretNotes.Views
{
    public partial class SignUpPage : ContentPage
    {
        readonly AuthViewModel auth;

        public SignUpPage()
        {
            auth = AuthViewModel.Instance;
            InitializeComponent();

            passLabel.Text = "Password must contain a lowercase " +
                "letter, an upper case letter, " +
                "a special character, and number!";
            passLabel.FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label));
            passLabel.Margin = new Thickness(20);
        }

        bool PasswordChecker(string pass)
        {
            Regex passRegex = new Regex(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$");
            Match passMatch = passRegex.Match(pass);
            return passMatch.Success;
        }

        async void SignUpClicked(object sender, EventArgs e)
        {
            if(PasswordChecker(PasswordInput.Text))
            {
                try
                {
                    await auth.SignUp(EmailInput.Text, PasswordInput.Text);

                    await DisplayAlert("Sign Up Completed!",
                        "You now have an account under the email, " +
                        $"\"{EmailInput.Text}!\"", "Sounds good!");
                    await Navigation.PopAsync();
                }
                catch
                {
                    ShowError();
                }
            }
            else
            {
                await DisplayAlert("Error!", "Password must contain" +
                    " a lowercase letter, an upper case letter, a special" +
                    " character, and number", "Try Again!");
            }
        }

        async private void ShowError()
        {
            await DisplayAlert("Sign Up Failed", "E-mail has already been used. Try again!", "OK");
        }
    }
}
