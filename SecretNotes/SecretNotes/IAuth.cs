using System;
using System.Threading.Tasks;

namespace SecretNotes
{
    public interface IAuth
    {
        Task<string> LoginWithEmailPassword(string email, string password);

        Task<string> GetTokenID();

        bool SignOut();
        bool IsSignedIn();

        Task SignUp(string email, string password);
    }
}
