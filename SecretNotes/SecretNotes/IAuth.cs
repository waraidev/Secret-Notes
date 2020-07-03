using System;
using System.Threading.Tasks;

namespace SecretNotes
{
    public interface IAuth
    {
        Task<string> LoginWithEmailPassword(string email, string password);

        bool SignOut();
        bool IsSignedIn();
    }
}
