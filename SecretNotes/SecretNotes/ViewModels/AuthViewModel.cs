using System;
using System.Threading.Tasks;
using SecretNotes;
using Xamarin.Forms;

namespace SecretNotes.ViewModels
{
    public class AuthViewModel
    {
        private static AuthViewModel _instance;

        public IAuth Auth { get; set; }

        public string Token { get; set; }

        public string Email { get; set; }

        public string Pass { get; set; }

        public static AuthViewModel Instance
        {
            get => _instance ?? new AuthViewModel();
        }

        public AuthViewModel()
        {
            Auth = DependencyService.Get<IAuth>();
        }

        public async Task<bool> IsTokenValid(string savedToken)
        {
            var fireToken = await Auth.GetTokenID();
            bool v = (savedToken == fireToken);
            return v;
        }

        public async void GetAuthToken(string email, string pass)
        {
            Email = email;
            Pass = pass;
            Token = await Auth.LoginWithEmailPassword(Email, Pass);
        }

        public bool SignOut()
        {
            return Auth.SignOut();
        }

        public async Task SignUp(string email, string pass)
        {
            Email = email;
            Pass = pass;
            await Auth.SignUp(Email, Pass);
        }
    }
}
