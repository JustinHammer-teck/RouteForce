// namespace RouteForce.Application.Service;
//
// public interface IPasswordHasher
// {
//     string HashPassword(string plainTextPassword);
//     bool VerifyHashedPassword(string hashedPassword, string plainTextPassword);
// }

using System.Security.Cryptography;
using System.Text;

namespace RouteForce.Application.Service
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var hashOfInput = HashPassword(providedPassword);
            return hashOfInput == hashedPassword;
        }
    }
}

// public interface IUserRepository
// {
//     Task<User?> FindByUsernameAsync(string username);
//     Task CreateUserAsync(User user);
//     IEnumerable<User> GetAllUsers();
// }

