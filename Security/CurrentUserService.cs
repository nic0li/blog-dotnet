using System.Security.Claims;
using Blog.Security.Interfaces;

namespace Blog.Security;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long GetUserId()
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            throw new UnauthorizedAccessException(
                "User not authenticated");
        }

        return long.Parse(userId);
    }
}