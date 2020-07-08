using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Firebase.Auth;
using SecretNotes.iOS;
using Foundation;

[assembly: Dependency(typeof(AuthIOS))]
namespace SecretNotes.iOS
{
    public class AuthIOS : IAuth
    {
        public async Task<User> LoginWithEmailPassword(string email, string password)
        {
            var user = await Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
            return user.User;
        }

        public async Task<string> GetToken(User authUser)
        {
            return await authUser.GetIdTokenAsync();
        }

        public async void UpdatePassword(string pass)
        {
            await Auth.DefaultInstance.CurrentUser.UpdatePasswordAsync(pass);
        }

        public bool IsSignedIn()
        {
            var user = Auth.DefaultInstance.CurrentUser;
            return user != null;
        }

        public bool SignOut()
        {
            try
            {
                _ = Auth.DefaultInstance.SignOut(out NSError error);
                return error == null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task SignUp(string email, string password)
        {
            await Auth.DefaultInstance.CreateUserAsync(email, password);
        }
    }
}