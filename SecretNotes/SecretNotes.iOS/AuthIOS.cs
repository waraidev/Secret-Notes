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
        public async Task<string> LoginWithEmailPassword(string email, string password)
        {
            try
            {
                var user = await Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
                return await user.User.GetIdTokenAsync();
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public async Task<string> GetTokenID()
        {
            var token = await Auth.DefaultInstance.CurrentUser.GetIdTokenAsync();
            return token;
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