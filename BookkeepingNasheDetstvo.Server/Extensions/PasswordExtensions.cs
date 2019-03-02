using System.Security.Cryptography;
using System.Text;

namespace BookkeepingNasheDetstvo.Server.Extensions
{
    public static class PasswordExtensions
    {
        public static string HashPassword(string password, string salt)
        {
            var passwordWithSaltBytes = Encoding.UTF8.GetBytes(string.Concat(password, salt));
            byte[] hashBytes;
            using (var hash = new SHA256Managed())
                hashBytes = hash.ComputeHash(passwordWithSaltBytes, 0, passwordWithSaltBytes.Length);
            return Encoding.UTF8.GetString(hashBytes);
        }

    }
}