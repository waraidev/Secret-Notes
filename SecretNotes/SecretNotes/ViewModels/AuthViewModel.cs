using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Firebase.Auth;
using Xamarin.Forms;

namespace SecretNotes.ViewModels
{
    public class AuthViewModel
    {
        private static AuthViewModel _instance;

        private static string _key;

        public IAuth Auth { get; set; }

        public string Token { get; set; }

        public User User { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// This uses the Singleton design pattern to
        /// make sure that all classes use the same instance
        /// </summary>
        public static AuthViewModel Instance
        {
            get
            {
                _instance ??= new AuthViewModel();
                return _instance;
            }
        }

        public AuthViewModel()
        {
            Auth = DependencyService.Get<IAuth>();
            GenerateKey();

            if(Application.Current.Properties.ContainsKey("token"))
                Token = (string)Application.Current.Properties["token"];

            _instance = this;
        }

        #region Non-Firebase Methods

        /// <summary>
        /// Generates a random key for the password
        /// encryption and decryption.
        /// </summary>
        private void GenerateKey()
        {
            var length = 7;

            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (var i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }

            _key = str_build.ToString();
        }

        /// <summary>
        /// Computes hash for encrypting email
        /// </summary>
        /// <param name="rawData"></param>
        public static string ComputeSha256Hash(string rawData) //Computing hash of email for security
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Returns true if string contains a special character,
        /// lower case letter, upper case letter, and a number.
        /// Must also have at least 8 letters
        /// </summary>
        /// <param name="pass"></param>
        public bool PasswordChecker(string pass)
        {

            Regex passRegex = new Regex(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$");
            Match passMatch = passRegex.Match(pass);
            return passMatch.Success;
        }

        public bool IteratedPasswordsCheck(string newPass)
        {
            string subPass1, subPass2, iterNewPass = newPass,
                oldPass = PasswordDecrypt(Password);

            if (iterNewPass.Length > oldPass.Length)
                iterNewPass = iterNewPass.Substring(0, oldPass.Length);
            else
                oldPass = oldPass.Substring(0, newPass.Length);

            for(var i = 0; i < newPass.Length - 5; i++)
            {
                subPass1 = oldPass.Substring(i, 5);
                subPass2 = iterNewPass.Substring(i, 5);
                if (subPass1 == subPass2)
                    return false;   //Fails check

                if (iterNewPass.Length == i + 5)
                    break;
            }

            return true; //Passes check
        }

        /// <summary>
        /// Encrypts the user's password using the
        /// Rfc2898DeriveBytes class.
        /// </summary>
        /// <param name="inText"></param>
        public string PasswordEncrypt(string inText)
        {
            byte[] bytesBuff = Encoding.Unicode.GetBytes(inText);
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(_key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Dispose();
                    }
                    inText = Convert.ToBase64String(mStream.ToArray());
                }
            }
            return inText;
        }

        /// <summary>
        /// Decrypts the user's password using the
        /// Rfc2898DeriveBytes class.
        /// </summary>
        /// <param name="cryptTxt"></param>
        public string PasswordDecrypt(string cryptTxt)
        {
            cryptTxt = cryptTxt.Replace(" ", "+");
            byte[] bytesBuff = Convert.FromBase64String(cryptTxt);
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(_key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Dispose();
                    }
                    cryptTxt = Encoding.Unicode.GetString(mStream.ToArray());
                }
            }
            return cryptTxt;
        }

        #endregion

        #region Firebase Methods

        /// <summary>
        /// Updates user password on Firebase
        /// </summary>
        /// <param name="pass"></param>
        public void UpdatePassword(string pass)
        {
            Auth.UpdatePassword(pass);
        }

        /// <summary>
        /// Gets auth token based on user login.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        public async void GetAuthToken(string email, string pass)
        {
            var user = await Auth.LoginWithEmailPassword(email, pass);
            Token = await Auth.GetToken(user) ?? Token;
        }

        /// <summary>
        /// Signs user out of Firebase
        /// </summary>
        public bool SignOut()
        {
            return Auth.SignOut();
        }

        /// <summary>
        /// Signs the user up for a new account.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        public async Task SignUp(string email, string pass)
        {
            await Auth.SignUp(email, pass);
        }

        #endregion
    }
}
