namespace Blog.Security.Interfaces;

public interface IJwtService
{
    string GenerateToken(long userId);

    long ExtractUserId(string token);
}