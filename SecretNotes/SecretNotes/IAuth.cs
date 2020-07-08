using System;
using System.Threading.Tasks;
using Firebase.Auth;

namespace SecretNotes
{
    /// <summary>
    /// AuthIOS implements this interface so that Auth functions can be within
    /// Xamarin shared code
    /// </summary>
    public interface IAuth
    {
        Task<User> LoginWithEmailPassword(string email, string password);

        void UpdatePassword(string pass);

        Task<string> GetToken(User authUser);

        bool SignOut();
        bool IsSignedIn();

        Task SignUp(string email, string password);
    }
}
