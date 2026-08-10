using BC = BCrypt.Net.BCrypt;
using Blog.Security.Interfaces;

namespace Blog.Security;

public class PasswordService : IPasswordService
{
    public string Hash(string password)
    {
        return BC.HashPassword(password);
    }

    public bool Verify(string password, string passwordHash)
    {
        return BC.Verify(password, passwordHash);
    }
}