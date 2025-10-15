namespace RouteForce.Application.Service;

public interface IPasswordHasher
{
    string HashPassword(string plainTextPassword);
    bool VerifyHashedPassword(string hashedPassword, string plainTextPassword);
}