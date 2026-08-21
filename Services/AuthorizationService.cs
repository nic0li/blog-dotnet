using Blog.Entities;
using Blog.Enums;
using Blog.Exceptions;
using Blog.Services.Interfaces;

namespace Blog.Services;

public class AuthorizationService(
    IAuthenticationService authenticationService) : IAuthorizationService
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<User> GetAuthenticatedUserAsync()
    {
        return await _authenticationService
            .GetAuthenticatedUserAsync();
    }

    public async Task<bool> IsOwnerAsync(User resourceOwner)
    {
        return IsOwner(resourceOwner, await GetAuthenticatedUserAsync());
    }

    public async Task<bool> IsAdminAsync()
    {
        return IsAdmin(await GetAuthenticatedUserAsync());
    }

    public async Task ValidateOwnerAsync(User resourceOwner)
    {
        if (!await IsOwnerAsync(resourceOwner))
        {
            throw NotAllowed();
        }
    }

    public async Task ValidateAdminAsync()
    {
        if (!await IsAdminAsync())
        {
            throw new ForbiddenException(
                "Administrator privileges required");
        }
    }

    public async Task ValidateOwnerOrAdminAsync(User resourceOwner)
    {
        var authenticatedUser = await GetAuthenticatedUserAsync();

        if (!IsOwner(resourceOwner, authenticatedUser) &&
            !IsAdmin(authenticatedUser))
        {
            throw NotAllowed();
        }
    }

    private static bool IsOwner(User resourceOwner, User authenticatedUser)
    {
        return resourceOwner.Id == authenticatedUser.Id;
    }

    private static bool IsAdmin(User authenticatedUser)
    {
        return authenticatedUser.Role == UserRole.Admin;
    }

    private static ForbiddenException NotAllowed()
    {
        throw new ForbiddenException(
            "You are not allowed to modify this resource");
    }
}