using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.AuthHelper
{
    public static class PasswordHelper
    {

        //  Hashing password fra  creating User or Opdatering User
        public static string Hash(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);

            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }


        // Den er tjekker faktisk opdetering hvis der er Admin
        public static bool Verify(string password, string storedHash)
        {
            var hashedInput = Hash(password);

            // DEBUG OUTPUT
            Console.WriteLine("[DEBUG] Raw Input Password: " + password);
            Console.WriteLine("[DEBUG] Hashed Input:       " + hashedInput);
            Console.WriteLine("[DEBUG] Stored Hash:        " + storedHash);

            return string.Equals(hashedInput, storedHash, StringComparison.OrdinalIgnoreCase);
        }

    }

}
